using System.Reflection;
using System.Security.Claims;
using AuthService.Authentication;
using AuthService.Services.Jwt;
using AuthService.Services.Account;
using Microsoft.AspNetCore.Authentication;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.OpenSearch;

var builder = WebApplication.CreateBuilder(args);

configureLogging();

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddTransient<IJwtService, JwtService>();
builder.Services.AddTransient<IAccountService, AccountService>();

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

builder.Host.UseSerilog();

var app = builder.Build();

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

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

