using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using BusinessLogicLayerCore.Services;
using BusinessLogicLayerCore.Services.Interfaces;
using DataAccessLayerCore;
using DataAccessLayerCore.Repositories;
using DataAccessLayerCore.Repositories.Interfaces;

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

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(connectionString)
);

// ======================
// JWT Configuration
// ======================
string? privateKeyPem = Environment.GetEnvironmentVariable("JWT_PRIVATE_KEY")
                        ?? File.ReadAllText(builder.Configuration["Jwt:PrivateKeyPem"] ?? string.Empty);

if (string.IsNullOrWhiteSpace(privateKeyPem))
    throw new ApplicationException("JWT private key is missing.");

RSA rsaPrivate = RSA.Create();
rsaPrivate.ImportFromPem(privateKeyPem.ToCharArray());
var rsaKey = new RsaSecurityKey(rsaPrivate);
var signingCredentials = new SigningCredentials(rsaKey, SecurityAlgorithms.RsaSha256);
builder.Services.AddSingleton(signingCredentials);

string? jwtIssuer = builder.Configuration["Authorization:Issuer"]
                    ?? throw new ApplicationException("Authorization:Issuer missing");
string? jwtAudience = builder.Configuration["Authorization:Audience"]
                      ?? throw new ApplicationException("Authorization:Audience missing");

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
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = rsaKey,
        ClockSkew = TimeSpan.FromMinutes(2)
    };
});

// ======================
// Email configuration
// ======================
var mailConnectionString = Environment.GetEnvironmentVariable("AppSettings_EmailSmtp")
                             ?? builder.Configuration["Email:ConnectionString"];
var mailSenderAddress = Environment.GetEnvironmentVariable("AppSettings_EmailFrom")
                         ?? builder.Configuration["Email:SenderAddress"];

if (!string.IsNullOrWhiteSpace(mailConnectionString) && !string.IsNullOrWhiteSpace(mailSenderAddress))
{
    builder.Services.AddScoped<IEmailService>(sp =>
        new EmailService(mailConnectionString, mailSenderAddress)
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

app.UseCors("FrontEndUI");

// Developer exception page for dev
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
