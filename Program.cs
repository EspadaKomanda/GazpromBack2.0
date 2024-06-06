using System.Security.Claims;
using AuthService.Authentication;
using AuthService.Services.Jwt;
using BackGazprom.Database;
using BackGazprom.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddTransient<IJwtService, JwtService>();

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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
