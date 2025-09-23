using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using BusinessLogicLayer.Services;
using BusinessLogicLayer.Services.Interfaces;
using DataAccessLayerCore.Repositories.Interfaces;
using DataAccessLayerCore.Repositories;
using DataAccessLayerCore;

// ======================
// Create builder
// ======================
var builder = WebApplication.CreateBuilder(args);

// ======================
// Configure port for Azure
// ======================
var portEnv = Environment.GetEnvironmentVariable("WEBSITES_PORT");
var port = string.IsNullOrWhiteSpace(portEnv) ? 8080 : int.Parse(portEnv);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(port);
});

// ======================
// Database configuration
// ======================
var connectionString = Environment.GetEnvironmentVariable("Data__ConnectionString");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new ApplicationException("Database connection string is missing. Set Data__ConnectionString in App Settings.");
}

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(connectionString)
);

// ======================
// JWT configuration with fallback (env or local file)
// ======================
string? privateKeyPem = Environment.GetEnvironmentVariable("JWT_PRIVATE_KEY");
string? publicKeyPem = Environment.GetEnvironmentVariable("JWT_PUBLIC_KEY"); // optional
string? jwtIssuer = Environment.GetEnvironmentVariable("Authorization_Issuer");
string? jwtAudience = Environment.GetEnvironmentVariable("Authorization_Audience");

// Fallback local file (development)
if (string.IsNullOrWhiteSpace(privateKeyPem))
{
    var pemPath = builder.Configuration["Jwt:PrivateKeyPem"];
    if (!string.IsNullOrEmpty(pemPath) && File.Exists(pemPath))
    {
        privateKeyPem = File.ReadAllText(pemPath);
    }
}

if (string.IsNullOrWhiteSpace(privateKeyPem))
{
    throw new ApplicationException("JWT signing key is not configured. Provide JWT_PRIVATE_KEY or Jwt:PrivateKeyPem file.");
}

// Import private key
RSA rsaPrivate = RSA.Create();
rsaPrivate.ImportFromPem(privateKeyPem.ToCharArray());
var rsaKey = new RsaSecurityKey(rsaPrivate);
var signingCredentials = new SigningCredentials(rsaKey, SecurityAlgorithms.RsaSha256);
builder.Services.AddSingleton(signingCredentials);

// ======================
// Service registrations
// ======================
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBaseRepository, BaseRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRegisterService, RegisterService>();
builder.Services.AddScoped<ILoginChecker, DbLoginChecker>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ======================
// Authentication setup
// ======================
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    SecurityKey issuerSigningKey;

    if (!string.IsNullOrWhiteSpace(publicKeyPem))
    {
        RSA rsaPub = RSA.Create();
        rsaPub.ImportFromPem(publicKeyPem.ToCharArray());
        issuerSigningKey = new RsaSecurityKey(rsaPub);
    }
    else
    {
        issuerSigningKey = rsaKey; // fallback la cheia privată
    }

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = issuerSigningKey,
        ClockSkew = TimeSpan.FromMinutes(2)
    };
});

// ======================
// Build app
// ======================
var app = builder.Build();

// ======================
// Middleware
// ======================
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(policy => policy
    .WithOrigins("http://localhost:7180", "http://localhost:4200", "http://localhost:5017")
    .AllowAnyHeader()
    .AllowAnyMethod()
);

app.UseAuthentication();
app.UseAuthorization();

// ======================
// Map controllers
// ======================
app.MapControllers();

// ======================
// Run app
// ======================
app.Run();
