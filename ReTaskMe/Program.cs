using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using BusinessLogicLayer.Services;
using BusinessLogicLayer.Services.Interfaces;

using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using DataAccessLayerCore;
using DataAccessLayerCore.Repositories;
using DataAccessLayerCore.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("MariaDbConnection"),
        new MySqlServerVersion(new Version(12, 0, 2)) 
    )
);

builder.Configuration.AddUserSecrets<Program>();
var jwtPrivateKeyPem = builder.Configuration["Jwt:PrivateKeyPem"]; // optional path to private key .pem
var jwtPublicKeyPem = builder.Configuration["Jwt:PublicKeyPem"]; // optional path to public key .pem
var jwtIssuer = builder.Configuration["Authorization:Issuer"];
var jwtAudience = builder.Configuration["Authorization:Audience"];
// Enforce certificate/PEM for signing

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// The Scoped lifetime means a new instance is created for each HTTP request.
builder.Services.AddScoped<IAuthService, AuthService>();

{
    SigningCredentials signingCredentials;

    if (!string.IsNullOrWhiteSpace(jwtPrivateKeyPem) && File.Exists(jwtPrivateKeyPem))
    {
        RSA rsa = RSA.Create();
        var privatePem = File.ReadAllText(jwtPrivateKeyPem);
        rsa.ImportFromPem(privatePem);
        var rsaKey = new RsaSecurityKey(rsa);
        signingCredentials = new SigningCredentials(rsaKey, SecurityAlgorithms.RsaSha256);
        builder.Services.AddSingleton(signingCredentials);
    }
    else
    {
        throw new ApplicationException("JWT PEM is not configured. Provide Jwt:PrivateKeyPem (PEM). Optional Jwt:PublicKeyPem for validation.");
    }
}

// Test reg
builder.Services.AddScoped<IBaseRepository, BaseRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRegisterService, RegisterService>();


// Register temporary login checker.
builder.Services.AddScoped<ILoginChecker, TemporaryLoginChecker>();

// Configure authentication with JWT Bearer.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})

.AddJwtBearer(options =>
{
    SecurityKey issuerSigningKey;
    if (!string.IsNullOrWhiteSpace(jwtPublicKeyPem) && File.Exists(jwtPublicKeyPem))
    {
        RSA rsaPub = RSA.Create();
        var pubPem = File.ReadAllText(jwtPublicKeyPem);
        rsaPub.ImportFromPem(pubPem);
        issuerSigningKey = new RsaSecurityKey(rsaPub);
    }
    else if (!string.IsNullOrWhiteSpace(jwtPrivateKeyPem) && File.Exists(jwtPrivateKeyPem))
    {
        RSA rsa = RSA.Create();
        var privatePem = File.ReadAllText(jwtPrivateKeyPem);
        rsa.ImportFromPem(privatePem);
        issuerSigningKey = new RsaSecurityKey(rsa);
    }
    else
    {
        throw new ApplicationException("JWT PEM is not configured. Provide Jwt:PublicKeyPem or Jwt:PrivateKeyPem (PEM).");
    }

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = issuerSigningKey
    };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
