using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using BusinessLogicLayer.Services;
using BusinessLogicLayer.Services.Interfaces;

using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

using DataAccessLayer.Repositories.Interfaces;
using DataAccessLayer.Repositories;
using DataAccessLayer;

var builder = WebApplication.CreateBuilder(args);

// Database configuration (supports InMemory for local testing)
var useInMemory = builder.Configuration.GetValue<bool>("Database:UseInMemory");
if (useInMemory)
{
    builder.Services.AddDbContext<DatabaseContext>(options => options.UseInMemoryDatabase("TestDb"));
}
else
{
    var connectionString = builder.Configuration.GetConnectionString("MariaDbConnection");
    builder.Services.AddDbContext<DatabaseContext>(options =>
        options.UseMySql(
            connectionString,
            ServerVersion.AutoDetect(connectionString)
        )
    );
}

builder.Configuration.AddUserSecrets<Program>();
var jwtSecret = builder.Configuration["JwtSecret"]; // symmetric fallback
var jwtPrivateKeyPem = builder.Configuration["Jwt:PrivateKeyPem"]; // raw PEM content or path
var jwtPublicKeyPem = builder.Configuration["Jwt:PublicKeyPem"]; // raw PEM content or path
var jwtIssuer = builder.Configuration["Authorization:Issuer"];
var jwtAudience = builder.Configuration["Authorization:Audience"];

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// The Scoped lifetime means a new instance is created for each HTTP request.
builder.Services.AddScoped<IAuthService, AuthService>();
// Provide SigningCredentials via DI (prefer RSA PEM, fallback to symmetric secret)
{
    SigningCredentials signingCredentials;

    RSA? rsa = null;
    if (!string.IsNullOrWhiteSpace(jwtPrivateKeyPem))
    {
        var pemContent = File.Exists(jwtPrivateKeyPem) ? File.ReadAllText(jwtPrivateKeyPem) : jwtPrivateKeyPem;
        try
        {
            rsa = RSA.Create();
            rsa.ImportFromPem(pemContent);
        }
        catch
        {
            rsa?.Dispose();
            rsa = null;
        }
    }

    if (rsa != null)
    {
        signingCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256);
    }
    else
    {
        if (string.IsNullOrEmpty(jwtSecret)) throw new ApplicationException("JwtSecret is not configured.");
        signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)), SecurityAlgorithms.HmacSha256);
    }

    builder.Services.AddSingleton(signingCredentials);
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
    // Build validation key (prefer public PEM, then private PEM, else symmetric)
    SecurityKey issuerSigningKey;
    RSA? rsaValidate = null;
    if (!string.IsNullOrWhiteSpace(jwtPublicKeyPem))
    {
        var pubContent = File.Exists(jwtPublicKeyPem) ? File.ReadAllText(jwtPublicKeyPem) : jwtPublicKeyPem;
        try { rsaValidate = RSA.Create(); rsaValidate.ImportFromPem(pubContent); } catch { rsaValidate = null; }
    }
    if (rsaValidate == null && !string.IsNullOrWhiteSpace(jwtPrivateKeyPem))
    {
        var privContent = File.Exists(jwtPrivateKeyPem) ? File.ReadAllText(jwtPrivateKeyPem) : jwtPrivateKeyPem;
        try { rsaValidate = RSA.Create(); rsaValidate.ImportFromPem(privContent); } catch { rsaValidate = null; }
    }
    if (rsaValidate != null)
    {
        issuerSigningKey = new RsaSecurityKey(rsaValidate);
    }
    else
    {
        if (string.IsNullOrEmpty(jwtSecret)) throw new ApplicationException("JwtSecret is not configured.");
        issuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
    }

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = !string.IsNullOrEmpty(jwtIssuer),
        ValidateAudience = !string.IsNullOrEmpty(jwtAudience),
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
