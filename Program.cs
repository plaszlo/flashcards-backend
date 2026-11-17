using Microsoft.AspNetCore.HttpOverrides;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Identity;
using FlashcardBackend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FlashcardBackend.Data.Interfaces;
using FlashcardBackend.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICardRepository, CardRepository>();

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", corsBuilder =>
    {
        corsBuilder.WithOrigins(
            "http://localhost:5173",
            "https://flashcards-frontend.netlify.app",
            "https://dev--flashcards-frontend.netlify.app"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

// Process forwarded headers from Render to identify the original client IP and HTTPS protocol
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:TokenKey").Value 
                    ?? throw new InvalidOperationException("JTW signing key is missing"))
            ),
            ValidateIssuer = true,
            ValidIssuer = "FlashcardAPI",

            ValidateAudience = true,
            ValidAudience = "FlashcardAPI",

            ValidateIssuerSigningKey = true,
            ValidateLifetime = true
        };
    });

var app = builder.Build();

app.UseForwardedHeaders(); // must be at the top of the pipeline

app.UseCors("CorsPolicy");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers(); 

app.Run();
