using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using BusinessLogicLayer.Services;
using BusinessLogicLayer.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using DataAccessLayerCore.Repositories.Interfaces;
using DataAccessLayerCore.Repositories;
using DataAccessLayerCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(
        Environment.GetEnvironmentVariable("Data__ConnectionString")
    )
);

builder.Configuration.AddUserSecrets<Program>();
//var jwtPrivateKeyPem = builder.Configuration["Jwt:PrivateKeyPem"]; // optional path to private key .pem
var jwtPrivateKeyPem = Environment.GetEnvironmentVariable("JWT_SECRET_KEY"); // Evironment variable declared in azure
//var jwtPublicKeyPem = builder.Configuration["Jwt:PublicKeyPem"]; // optional path to public key .pem
string? jwtPublicKeyPem = null;
// Enforce RSA PEM only (no symmetric secret)
var jwtIssuer = builder.Configuration["Authorization:Issuer"];
var jwtAudience = builder.Configuration["Authorization:Audience"];

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
        throw new ApplicationException("JWT signing is not configured. Provide Jwt:PrivateKeyPem (PEM).");
    }
}

// Test reg
builder.Services.AddScoped<IBaseRepository, BaseRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRegisterService, RegisterService>();


// Register DB-backed login checker.
builder.Services.AddScoped<ILoginChecker, DbLoginChecker>();

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
        throw new ApplicationException("JWT validation key is not configured. Provide Jwt:PublicKeyPem or Jwt:PrivateKeyPem (PEM).");
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

var app = builder.Build();

app.UseSwagger();
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(configurePolicy => configurePolicy
    .WithOrigins("http://localhost:7180", "http://localhost:4200", "http://localhost:5017")
    .AllowAnyHeader()
    .AllowAnyMethod());
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
