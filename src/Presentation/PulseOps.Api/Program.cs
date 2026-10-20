using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PulseOps.Application.Common.Interfaces;
using PulseOps.Infrastructure.Persistence;
using PulseOps.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Controller Servisleri
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 2. JWT Authentication & Default Scheme Ayarları
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"] ?? "PulseOpsApi",
        ValidAudience = builder.Configuration["JwtSettings:Audience"] ?? "PulseOpsClient",
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"] ?? "super_secret_key_pulseops_123456789_secure"))
    };
});

// 3. Swagger ve JWT Bearer Desteği Yapılandırması
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PulseOps API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Token değerinizi 'Bearer {token}' formatında girin."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// 4. Veritabanı Bağlantı Kaydı (PostgreSQL - Port 5434)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Host=localhost;Port=5434;Database=pulseops_db;Username=postgres;Password=postgres";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// 5. Servis Kayıtları (Dependency Injection)
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

// 6. Background Monitoring Worker Kaydı
builder.Services.AddHostedService<MonitoringBackgroundService>();

var app = builder.Build();

// 7. HTTP Request Pipeline Yapılandırması
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Sıralama önemlidir: Önce Authentication, sonra Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();