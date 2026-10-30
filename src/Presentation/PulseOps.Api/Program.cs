using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PulseOps.Application.Common.Interfaces;
using PulseOps.Infrastructure.Persistence;
using PulseOps.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 2. JWT authentication and default scheme
// The signing key never lives in the repository: set JwtSettings__Secret (at least 32 bytes).
// In Development a random key is generated on each start, so tokens stop working after a restart.
var jwtSecret = builder.Configuration["JwtSettings:Secret"];
if (string.IsNullOrEmpty(jwtSecret))
{
    if (!builder.Environment.IsDevelopment())
        throw new InvalidOperationException("JwtSettings:Secret is not set. Create one with `openssl rand -base64 32`.");

    jwtSecret = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    builder.Configuration["JwtSettings:Secret"] = jwtSecret;
}
if (Encoding.UTF8.GetByteCount(jwtSecret) < 32)
    throw new InvalidOperationException("JwtSettings:Secret must be at least 32 bytes long.");

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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
    };
});

// 3. Swagger with JWT bearer support
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
        Description = "Enter your JWT as 'Bearer {token}'."
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

// 4. Database (PostgreSQL); override with ConnectionStrings__DefaultConnection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is not set.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// 5. Service registrations (dependency injection)
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

// 6. Background monitoring worker
builder.Services.AddHostedService<MonitoringBackgroundService>();

var app = builder.Build();

// 7. HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Order matters: authentication before authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();