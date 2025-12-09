using Azure.Communication.Email;
using BusinessLogicLayerCore.Services;
using BusinessLogicLayerCore.Services.Interfaces;
using DataAccessLayerCore;
using DataAccessLayerCore.Repositories;
using DataAccessLayerCore.Repositories.Interfaces;
using HelperLayer.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Cryptography;

// ======================
// Create builder
// ======================
var builder = WebApplication.CreateBuilder(args);

// ======================
// Database configuration
// ======================
var connectionString = Environment.GetEnvironmentVariable("Data__ConnectionString")
                       ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
    throw new ApplicationException("Database connection string is missing.");

// Register DbContext (was missing)
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(
        connectionString,
        b => b.MigrationsAssembly("ReTaskMe")   // <-- add this
    )
);


// ======================
// JWT Configuration
// ======================

var privateKeyPath = "private_key.pem";
var publicKeyPath = "public_key.pem";

// Load private key (env → file → appsettings)
string? privateKeyPem =
    Environment.GetEnvironmentVariable("JWT_PRIVATE_KEY")
    ?? File.ReadAllText(builder.Configuration["Jwt:PrivateKeyPem"] ?? privateKeyPath);
string? publicKeyPem =
    Environment.GetEnvironmentVariable("JWT_PUBLIC_KEY")
    ?? File.ReadAllText(builder.Configuration["Jwt:PublicKeyPem"] ?? publicKeyPath);

// Final check
if (string.IsNullOrWhiteSpace(privateKeyPem))
    throw new ApplicationException("JWT private key is missing.");

RSA rsaPrivate = RSA.Create();
RSA rsaPublic = RSA.Create();
rsaPrivate.ImportFromPem(privateKeyPem.ToCharArray());
rsaPublic.ImportFromPem(publicKeyPem.ToCharArray());
var rsaPrivateKey = new RsaSecurityKey(rsaPrivate);
var signingCredentials = new SigningCredentials(rsaPrivateKey, SecurityAlgorithms.RsaSha256);
builder.Services.AddSingleton(signingCredentials);


string? jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new ApplicationException("Jwt:Issuer missing");
string? jwtAudience = builder.Configuration["Jwt:Audience"] ?? throw new ApplicationException("Jwt:Audience missing");

Console.WriteLine($"Issuer from config: '{jwtIssuer}'");
Console.WriteLine($"Audience from config: '{jwtAudience}'");

var rsaPublicKey = new RsaSecurityKey(rsaPublic);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("JWT auth failed: " + context.Exception);
            return Task.CompletedTask;
        }
    };


    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = rsaPublicKey,   // use public key here
        ClockSkew = TimeSpan.FromMinutes(5)
    };
});


// ======================
// Email configuration
// ======================
var mailConnectionString = Environment.GetEnvironmentVariable("AppSettings_EmailSmtp")
                             ?? builder.Configuration["Email:ConnectionString"];
var mailSenderAddress = Environment.GetEnvironmentVariable("AppSettings_EmailFrom")
                         ?? builder.Configuration["Email:SenderAddress"];

System.Console.WriteLine(mailConnectionString);
System.Console.WriteLine(mailSenderAddress);
System.Console.WriteLine(mailConnectionString);

if (!string.IsNullOrWhiteSpace(mailConnectionString) && !string.IsNullOrWhiteSpace(mailSenderAddress))
{
    builder.Services.AddScoped<IEmailService>(sp =>
        new EmailService(mailSenderAddress)
    );
}
else
{
    builder.Services.AddScoped<IEmailService, NoOpEmailService>();
}

// ======================
// Repositories
// ======================
builder.Services.AddScoped<IBaseRepository, BaseRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserSessionRepository, UserSessionRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IBoardRepository, BoardRepository>();
builder.Services.AddScoped<ILoginChecker, LoginChecker>();

// ======================
// Business Services
// ======================
builder.Services.AddScoped<IRegisterService, RegisterService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IProfileService, ProfileService>();

// ======================
// Swagger
// ======================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Chan Kazan Nvidia top",
        Version = "v1",
        Description = "List of APIs"
    });
    o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
            "JWT Authorization header using bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    o.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });
});

// ======================
// CORS
// ======================
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontEndUI", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
    );
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("OpenCorsNoLimitation", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod()
    );
});

// ======================
// Controllers & Swagger
// ======================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ======================
// Build app
// ======================
var app = builder.Build();

app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI();


app.UseCors("OpenCorsNoLimitation");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
