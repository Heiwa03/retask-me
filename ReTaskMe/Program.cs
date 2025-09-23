// Microsoft Packages
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;


// System
using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;


// BL
using BusinessLogicLayerCore.Services;
using BusinessLogicLayerCore.Services.Interfaces;


// DAL
using DataAccessLayerCore.Repositories.Interfaces;
using DataAccessLayerCore.Repositories;
using DataAccessLayerCore;


// ---- PROGRAM.CS ------

// Builder
var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetValue<string>("ConnectionStrings:AzureSqlConnection") ?? throw new InvalidOperationException(), b => 
        {
            b.MigrationsAssembly("ReTaskMe");
            b.CommandTimeout(60);
        }
    )
);


builder.Configuration.AddUserSecrets<Program>();
var jwtPrivateKeyPem = builder.Configuration["JwtSecret:PrivateKeyPem"]; 
var jwtPublicKeyPem = builder.Configuration["Jwt:PublicKeyPem"]; 
var jwtIssuer = builder.Configuration["Authorization:Issuer"];
var jwtAudience = builder.Configuration["Authorization:Audience"];


// Enforce certificate/PEM for signing
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// The Scoped lifetime means a new instance is created for each HTTP request.

//builder.Services.AddScoped<IAuthService, AuthService>();

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
builder.Services.AddScoped<ITaskRepository, TaskRepository>();


builder.Services.AddScoped<IBaseRepository, BaseRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRegisterService, RegisterService>();

builder.Services.AddScoped<IUserService, UserService>();


// Register temporary login checker.
//builder.Services.AddScoped<ILoginChecker, TemporaryLoginChecker>();

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

// Task
builder.Services.AddScoped<ITaskService, TaskService>();


// App runner
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    db.Database.Migrate(); 
}

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
