using System.Reflection;
using System.Security.Claims;
using DialogService.Database;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.OpenSearch;
using DialogService.Repositories;
using DialogService.Services.DialogsService;
using DialogService.Services.MessagesService;
using Confluent.Kafka;

var builder = WebApplication.CreateBuilder(args);

configureLogging();

builder.Configuration.AddEnvironmentVariables();
builder.Services.AddSingleton(new ProducerBuilder<string,string>(
    new ProducerConfig()
    {
        BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BROKERS") ?? "localhost:29092",
        Partitioner = Partitioner.Murmur2,
        CompressionType = Confluent.Kafka.CompressionType.None,
        ClientId= Environment.GetEnvironmentVariable("KAFKA_CLIENT_ID"),
        
    }
).Build());

builder.Services.AddSingleton(new ConsumerBuilder<string,string>(
    new ConsumerConfig()
    {
        BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BROKERS") ?? "localhost:29092",
        GroupId = Environment.GetEnvironmentVariable("KAFKA_GROUP_ID") ?? "dialog-service", 
        EnableAutoCommit = true,
        AutoCommitIntervalMs = 10,
        EnableAutoOffsetStore = true,
        AutoOffsetReset = AutoOffsetReset.Latest
    }
).Build());
builder.Services.AddSingleton(new AdminClientBuilder(
    new AdminClientConfig()
    {
        BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BROKERS") ?? "localhost:29092"
    }
).Build());
// Database
builder.Services.AddDbContext<ApplicationContext>(x => {
    var Hostname=Environment.GetEnvironmentVariable("DB_HOSTNAME") ?? "localhost";
    var Port=Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
    var Name=Environment.GetEnvironmentVariable("DB_NAME") ?? "postgres";
    var Username=Environment.GetEnvironmentVariable("DB_USERNAME") ?? "postgres";
    var Password=Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "postgres";
    x.UseNpgsql($"Server={Hostname}:{Port};Database={Name};Uid={Username};Pwd={Password};");
});

// Services
builder.Services.AddTransient<IDialogRepository, DialogRepository>();
builder.Services.AddTransient<IMessageRepository, MessageRepository>();
builder.Services.AddTransient<IDialogsService, DialogsService>();
builder.Services.AddTransient<IMessagesService, MessagesService>();



builder.Services.AddControllers();

builder.Host.UseSerilog();

var app = builder.Build();

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();

void configureLogging(){
    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
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

