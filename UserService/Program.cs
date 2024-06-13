using System.Reflection;
using UserService.Database;
using UserService.Repositories;
using UserService.Services.Account;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.OpenSearch;

var builder = WebApplication.CreateBuilder(args);

configureLogging();

builder.Configuration.AddEnvironmentVariables();

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
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IUserProfileRepository, UserProfileRepository>();
builder.Services.AddTransient<IRoleRepository, RoleRepository>();
builder.Services.AddTransient<IRegistrationCodeRepository, RegistrationCodeRepository>();
builder.Services.AddTransient<IRegistrationCodeRepository, RegistrationCodeRepository>();
builder.Services.AddTransient<IAccountService, AccountService>();

// Logs
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

