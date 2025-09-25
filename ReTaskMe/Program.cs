using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using BusinessLogicLayerCore.Services;
using BusinessLogicLayerCore.Services.Interfaces;
using DataAccessLayerCore;
using DataAccessLayerCore.Repositories;
using DataAccessLayerCore.Repositories.Interfaces;
using HelperLayer.Security;
using Azure.Communication.Email;

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
    throw new ApplicationException("Database connection string is missing. Set Data__ConnectionString in App Settings.");

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(connectionString)
);

// ======================
// JWT configuration
// ======================
var privateKeyPem = Environment.GetEnvironmentVariable("JWT_PRIVATE_KEY") 
                    ?? builder.Configuration["Jwt:PrivateKeyPem"];

if (string.IsNullOrWhiteSpace(privateKeyPem))
    throw new ApplicationException("JWT private key is missing. Set JWT_PRIVATE_KEY or Jwt:PrivateKeyPem in configuration.");

RSA rsaPrivate = RSA.Create();
rsaPrivate.ImportFromPem(privateKeyPem.ToCharArray());
var rsaKey = new RsaSecurityKey(rsaPrivate);
builder.Services.AddSingleton(new SigningCredentials(rsaKey, SecurityAlgorithms.RsaSha256));

var jwtIssuer = Environment.GetEnvironmentVariable("Authorization_Issuer") 
                ?? builder.Configuration["Jwt:Issuer"];
var jwtAudience = Environment.GetEnvironmentVariable("Authorization_Audience") 
                  ?? builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtIssuer) || string.IsNullOrWhiteSpace(jwtAudience))
    throw new ApplicationException("JWT issuer or audience is missing.");

// ======================
// Email configuration
// ======================
var mailConnectionString = Environment.GetEnvironmentVariable("AppSettings_EmailSmtp");
var mailSenderAddress = Environment.GetEnvironmentVariable("AppSettings_EmailFrom");

if (string.IsNullOrWhiteSpace(mailConnectionString) || string.IsNullOrWhiteSpace(mailSenderAddress))
    throw new ApplicationException("Email configuration missing. Set AppSettings_EmailSmtp and AppSettings_EmailFrom in App Settings.");

builder.Services.AddSingleton(sp =>
{
    var client = new EmailClient(mailConnectionString);
    return new EmailHelper(client, mailSenderAddress);
});
builder.Services.AddScoped<IEmailService, EmailService>();

// ======================
// Application services
// ======================
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBaseRepository, BaseRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserSessionRepository, UserSessionRepository>();
builder.Services.AddScoped<IRegisterService, RegisterService>();
builder.Services.AddScoped<ILoginChecker, LoginChecker>();

// ======================
// CORS configuration
// ======================
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontEndUI", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://red-mud-060e13903.1.azurestaticapps.net")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ======================
// Controllers & Swagger
// ======================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ======================
// JWT Authentication
// ======================
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
        IssuerSigningKey = rsaKey,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        ClockSkew = TimeSpan.FromMinutes(2)
    };
});

// ======================
// Build & run app
// ======================
var app = builder.Build();

app.UseCors("FrontEndUI");

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
