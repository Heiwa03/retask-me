using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using BusinessLogicLayer.Services;
using DataAccessLayerCore;
using DataAccessLayerCore.Repositories;
using HelperLayer.Security;
using Azure.Communication.Email;
using BusinessLogicLayer.Services.Interfaces;
using DataAccessLayerCore.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;


var builder = WebApplication.CreateBuilder(args);

// --- Database ---
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AzureConnection"))
);

// --- JWT Authentication ---
builder.Configuration.AddUserSecrets<Program>();
builder.Configuration.AddEnvironmentVariables();

var jwtPrivateKeyPem = builder.Configuration["Jwt:PrivateKeyPem"]
                       ?? Environment.GetEnvironmentVariable("JWT_PRIVATE_KEY_PEM");
var jwtPublicKeyPem = builder.Configuration["Jwt:PublicKeyPem"]
                      ?? Environment.GetEnvironmentVariable("JWT_PUBLIC_KEY_PEM");
var jwtIssuer = builder.Configuration["Authorization:Issuer"];
var jwtAudience = builder.Configuration["Authorization:Audience"];

SigningCredentials signingCredentials;
if (!string.IsNullOrWhiteSpace(jwtPrivateKeyPem) && File.Exists(jwtPrivateKeyPem))
{
    var rsa = RSA.Create();
    rsa.ImportFromPem(File.ReadAllText(jwtPrivateKeyPem));
    signingCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256);
    builder.Services.AddSingleton(signingCredentials);
}
else
{
    throw new ApplicationException("JWT signing key not configured");
}

// --- Repositories & Services ---
builder.Services.AddScoped<IBaseRepository, BaseRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRegisterService, RegisterService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ILoginChecker, DbLoginChecker>();

// Email
var mailConnectionString = builder.Configuration["Email:ConnectionString"]
                           ?? Environment.GetEnvironmentVariable("EMAIL_CONNECTION_STRING");
var mailSenderAddress = builder.Configuration["Email:SenderAddress"]
                        ?? Environment.GetEnvironmentVariable("EMAIL_SENDER_ADDRESS");

if (!string.IsNullOrWhiteSpace(mailConnectionString) && !string.IsNullOrWhiteSpace(mailSenderAddress))
{
    builder.Services.AddSingleton(sp =>
    {
        var client = new EmailClient(mailConnectionString);
        return new EmailHelper(client, mailSenderAddress);
    });
}

// --- Controllers & Swagger ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- JWT Bearer ---
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
        var rsaPub = RSA.Create();
        rsaPub.ImportFromPem(File.ReadAllText(jwtPublicKeyPem));
        issuerSigningKey = new RsaSecurityKey(rsaPub);
    }
    else if (!string.IsNullOrWhiteSpace(jwtPrivateKeyPem) && File.Exists(jwtPrivateKeyPem))
    {
        var rsa = RSA.Create();
        rsa.ImportFromPem(File.ReadAllText(jwtPrivateKeyPem));
        issuerSigningKey = new RsaSecurityKey(rsa);
    }
    else
    {
        throw new ApplicationException("JWT validation key not configured");
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

// --- Middleware ---
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
