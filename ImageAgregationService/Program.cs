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
    options.Configuration = "83.166.239.45";
    options.InstanceName = "image_aggregation";
});
builder.Services.AddSingleton(new ProducerBuilder<string,string>(
    new ProducerConfig()
    {
        BootstrapServers = "90.156.218.15:29092",
        Partitioner = Partitioner.Murmur2,
        CompressionType = Confluent.Kafka.CompressionType.None,
        ClientId="image-producer"
    }
).Build());

builder.Services.AddSingleton(new ConsumerBuilder<string,string>(
    new ConsumerConfig()
    {
        BootstrapServers = "90.156.218.15:29092",
        GroupId = "image-consumer", 
        EnableAutoCommit = true,
        AutoCommitIntervalMs = 10,
        EnableAutoOffsetStore = true,
        AutoOffsetReset = AutoOffsetReset.Latest
    }
).Build());
builder.Services.AddSingleton(new AdminClientBuilder(
    new AdminClientConfig()
    {
        BootstrapServers = "90.156.218.15:29092"
    }
).Build());
builder.Services.AddSingleton<IAmazonS3>(sc =>
{
    var awsS3Config = new AmazonS3Config
    {
        RegionEndpoint = RegionEndpoint.USEast1,
        ServiceURL = "http://83.166.237.29:9000",
        ForcePathStyle = true
    };

    return new AmazonS3Client("s3manager","s3manager",awsS3Config);
})
.AddSingleton<ConfigReader>();
builder.Services.AddDbContext<ApplicationContext>(x => {
    var Hostname="83.166.239.45";
    var Port="5432";
    var Name="imageagregationdb";
    var Username="postgres";
    var Password="QWERTYUIO2313";
    x.UseNpgsql($"Server={Hostname}:{Port};Database={Name};Uid={Username};Pwd={Password};");
});
builder.Services.AddSingleton<ImageGenerationCommunicator>();
builder.Services.AddSingleton<ImageVerifierCommunicator>();
builder.Services.AddSingleton<ImageTextAdderCommunicator>();
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
