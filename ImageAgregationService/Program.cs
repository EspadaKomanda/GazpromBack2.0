using System.Reflection;
using Amazon;
using Amazon.S3;
using Confluent.Kafka;
using ImageAgregationService.Database;
using ImageAgregationService.Repository;
using ImageAgregationService.Repository.ImageRepository;
using ImageAgregationService.Repository.MarkRepository;
using ImageAgregationService.Services;
using ImageAgregationService.Services.ImageAgregationService;
using ImageAgregationService.Services.MarkService;
using ImageAgregationService.Services.TemplateService;
using ImageAgregationService.Singletones;
using ImageAgregationService.Singletones.Communicators;
using KafkaTestLib.Kafka;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.OpenSearch;

var builder = WebApplication.CreateBuilder(args);
configureLogging();

// Add services to the container.
builder.Services.AddStackExchangeRedisCache(options => {
    options.Configuration =  Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING") ?? "localhost:6379";
    options.InstanceName =  Environment.GetEnvironmentVariable("REDIS_INSTANCE_NAME") ?? "default";
});
builder.Services.AddSingleton(new ProducerBuilder<string,string>(
    new ProducerConfig()
    {
        BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BROKERS") ?? "",
        Partitioner = Partitioner.Murmur2,
        CompressionType = Confluent.Kafka.CompressionType.None,
        ClientId= Environment.GetEnvironmentVariable("KAFKA_CLIENT_ID")
    }
).Build());

builder.Services.AddSingleton(new ConsumerBuilder<string,string>(
    new ConsumerConfig()
    {
        BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BROKERS") ?? "",
        GroupId = Environment.GetEnvironmentVariable("KAFKA_GROUP_ID") ?? "", 
        EnableAutoCommit = true,
        AutoCommitIntervalMs = 10,
        EnableAutoOffsetStore = true,
        AutoOffsetReset = AutoOffsetReset.Latest
    }
).Build());
builder.Services.AddSingleton(new AdminClientBuilder(
    new AdminClientConfig()
    {
        BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BROKERS") ?? ""
    }
).Build());
builder.Services.AddSingleton<IAmazonS3>(sc =>
{
    var awsS3Config = new AmazonS3Config
    {
        RegionEndpoint = RegionEndpoint.USEast1,
        ServiceURL = Environment.GetEnvironmentVariable("S3_URL") ?? "",
        ForcePathStyle = true
    };

    return new AmazonS3Client("s3manager","s3manager",awsS3Config);
})
.AddSingleton<ConfigReader>();
builder.Services.AddDbContext<ApplicationContext>(x => {
    var Hostname=Environment.GetEnvironmentVariable("DB_HOSTNAME") ?? "localhost";
    var Port=Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
    var Name=Environment.GetEnvironmentVariable("DB_NAME") ?? "postgres";
    var Username=Environment.GetEnvironmentVariable("DB_USERNAME") ?? "postgres";
    var Password=Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "postgres";
    x.UseNpgsql($"Server={Hostname}:{Port};Database={Name};Uid={Username};Pwd={Password};");
});
builder.Services.AddSingleton<ImageGenerationCommunicator>();
builder.Services.AddSingleton<ImageProcessorCommunicator>();
builder.Services.AddTransient<IS3Service, S3Service>();
builder.Services.AddScoped<ITemplateRepository, TemplateRepository>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<IMarkRepository, MarkRepository>();
builder.Services.AddTransient<IImageAgregationService, ImageAgregationService.Services.ImageAgregationService.ImageAgregationService>();
builder.Services.AddTransient<ITemplateService, TemplateService>();
builder.Services.AddTransient<IMarkService, MarkService>();
builder.Services.AddSingleton<KafkaTopicManager>();

builder.Host.UseSerilog();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<KafkaService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

Thread thread = new(async () => {
    var s3Service = app.Services.GetRequiredService<IS3Service>();
    await s3Service.ConfigureBuckets();
    using var scope = app.Services.CreateScope();
    var templateRepository = scope.ServiceProvider.GetRequiredService<ITemplateRepository>();
    var ConfigReader = app.Services.GetRequiredService<ConfigReader>();
    List<string> buckets = await ConfigReader.GetBuckets();
    await templateRepository.GenerateTemplates(buckets);
    var kafkaService = scope.ServiceProvider.GetRequiredService<KafkaService>();
    await kafkaService.Consume();
});

app.Run();
void configureLogging(){
    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
    var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json",optional:false,reloadOnChange:true).Build();
    Console.WriteLine(environment);
    Console.WriteLine(configuration);
    Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .WriteTo.Debug()
            .WriteTo.Console()
            .WriteTo.OpenSearch(ConfigureOpenSearchSink(configuration,environment))
            .Enrich.WithProperty("Environment",environment)
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
}
OpenSearchSinkOptions ConfigureOpenSearchSink(IConfiguration configuration,string environment){
    return new OpenSearchSinkOptions(new Uri(configuration["OpenSearchConfiguration:Uri"]!))
    {
        AutoRegisterTemplate = true,
        IndexFormat =  $"{Assembly.GetExecutingAssembly().GetName().Name!.ToLower().Replace(".","-")}-{environment.ToLower()}-{DateTime.UtcNow:yyyy-MM-DD}",
        NumberOfReplicas =1,
        NumberOfShards = 1
    };
}
