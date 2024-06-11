using System.Reflection;
using System.Security.Claims;
using AuthService.Authentication;
using AuthService.Services.Jwt;
using AuthService.Database;
using AuthService.Repositories;
using AuthService.Services.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.OpenSearch;
using Microsoft.OpenApi.Models;

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

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "AuthService",
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Аутентификация при помощи токена типа Access и Refresh.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        BearerFormat = "<type> <token>",
        Type = SecuritySchemeType.ApiKey
    });

    var xmlFile = Path.Combine(AppContext.BaseDirectory, "TestAPI.xml");
    if (File.Exists(xmlFile))
    {
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    }
});

builder.Host.UseSerilog();

var app = builder.Build();

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "/Auth/swagger/{documentName}/swagger.json";
    });
    app.UseSwaggerUI(c =>
    {
        c.RoutePrefix = "Auth";
        c.SwaggerEndpoint("/Auth/swagger/v1/swagger.json", "v1");
    });
}
else
{
    Console.WriteLine("Not Development");

}

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

