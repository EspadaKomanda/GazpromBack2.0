using System.Reflection;
using System.Security.Claims;
using ApiGatewayService.Services.Accont;
using ApiGatewayService.Services.ImageAgregationService;
using AuthService.Authentication;
using AuthService.Services.Account;
using AuthService.Services.Auth;
using AuthService.Services.Jwt;
using Confluent.Kafka;
using DialogService.Services.DialogsService;
using DialogService.Services.MessagesService;
using ImageAgregationService.Services.MarkService;
using ImageAgregationService.Services.TemplateService;
using KafkaTestLib.Kafka;
using Microsoft.AspNetCore.Authentication;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.OpenSearch;
using UserService.Services;
using UserService.Services.Account;
using UserService.Services.Roles;
using UserService.Services.UserInfoService;

var builder = WebApplication.CreateBuilder(args);

configureLogging();
builder.Host.UseSerilog();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(new ProducerBuilder<string,string>(
    new ProducerConfig()
    {
        BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BROKERS") ?? "",
        Partitioner = Partitioner.Murmur2,
        CompressionType = Confluent.Kafka.CompressionType.None,
        ClientId= Environment.GetEnvironmentVariable("KAFKA_CLIENT_ID") ?? ""
    }
).Build());

builder.Services.AddSingleton(new ConsumerBuilder<string,string>(
    new ConsumerConfig()
    {
        BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BROKERS") ?? "",
        GroupId = Environment.GetEnvironmentVariable("KAFKA_CLIENT_ID") ?? "", 
        EnableAutoCommit = true,
        AutoCommitIntervalMs = 10,
        EnableAutoOffsetStore = true,
        AutoOffsetReset = AutoOffsetReset.Latest
    }
).Build());
builder.Services.AddSingleton(new AdminClientBuilder(
    new AdminClientConfig()
    {
        BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BROKERS")
    }
).Build());
builder.Services.AddSingleton<KafkaService>()
                .AddSingleton<KafkaTopicManager>();
builder.Services.AddTransient<IUserService, UserService.Services.UserInfoService.UserService>()
                .AddTransient<ITemplateService, TemplateService>()
                .AddTransient<IImageAgregationService,ApiGatewayService.Services.ImageAgregationService.ImageAgregationService>()
                .AddTransient<IAccountService, AccountService>()
                .AddTransient<IAuthService, AuthService.Services.Auth.AuthService>()
                .AddTransient<IDialogsService, DialogService.Services.DialogsService.DialogService>()
                .AddTransient<IMarkService, MarkService>()
                .AddTransient<IJwtService, JwtService>()
                .AddTransient<IMessagesService, MessageService>()
                .AddTransient<IRolesService, RolesService>();

                
builder.Services.AddControllers();

// Authorization
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Access", policy =>
    {
        policy.RequireClaim(ClaimTypes.AuthenticationMethod, "Access");
    })
    .AddPolicy("Refresh", policy =>
    {
        policy.RequireClaim(ClaimTypes.AuthenticationMethod, "Refresh");
    });
    
builder.Services.AddAuthentication("default")
.AddScheme<AuthenticationSchemeOptions, JwtAuthenticationHandler>("default", options => 
{
    Console.WriteLine(options.ToString());
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapControllers();
app.UseHttpsRedirection();


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

