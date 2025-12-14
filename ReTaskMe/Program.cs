using Azure.Communication.Email;
using BusinessLogicLayerCore.Services;
using BusinessLogicLayerCore.Services.Interfaces;
using DataAccessLayerCore;
using DataAccessLayerCore.Repositories;
using DataAccessLayerCore.Repositories.Interfaces;
using HelperLayer.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Cryptography;
using OpenAI;
using BusinessLogicLayerCore.Services.SearchBehaviour;
using BusinessLogicLayerCore.Services.AiAgentBehaviour;

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

if (!File.Exists(privateKeyPath) || !File.Exists(publicKeyPath))
{
    Console.WriteLine("Generating RSA key pair...");
    using var rsa = RSA.Create(2048);
    
    var privateKey = rsa.ExportRSAPrivateKeyPem();
    File.WriteAllText(privateKeyPath, privateKey);
    
    var publicKey = rsa.ExportRSAPublicKeyPem();
    File.WriteAllText(publicKeyPath, publicKey);
    
    Console.WriteLine($"Created key pair: {privateKeyPath} (private), {publicKeyPath} (public)");
}

string privateKeyPem = File.ReadAllText(privateKeyPath).Replace("\\n", "\n").Trim();
Console.WriteLine($"Private key loaded from: {privateKeyPath}");

string publicKeyPem = File.ReadAllText(publicKeyPath).Replace("\\n", "\n").Trim();
Console.WriteLine($"Public key loaded from: {publicKeyPath}");

RSA rsaPrivate = RSA.Create();
RSA rsaPublic = RSA.Create();

rsaPrivate.ImportFromPem(privateKeyPem.ToCharArray());
rsaPublic.ImportFromPem(publicKeyPem.ToCharArray());

var rsaPrivateKey = new RsaSecurityKey(rsaPrivate);  
var rsaPublicKey = new RsaSecurityKey(rsaPublic);    

var signingCredentials = new SigningCredentials(rsaPrivateKey, SecurityAlgorithms.RsaSha256);
builder.Services.AddSingleton(signingCredentials);

Console.WriteLine("✓ RSA keys loaded successfully:");
Console.WriteLine($"  Private key (for signing): {rsaPrivateKey.KeySize} bits");
Console.WriteLine($"  Public key (for validation): {rsaPublicKey.KeySize} bits");

string jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "ReTaskMe";
string jwtAudience = builder.Configuration["Jwt:Audience"] ?? "ReTaskMeClients";

Console.WriteLine($"  Issuer: '{jwtIssuer}'");
Console.WriteLine($"  Audience: '{jwtAudience}'");

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
        IssuerSigningKey = rsaPublicKey,  
        ClockSkew = TimeSpan.FromMinutes(2)
    };
    
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            Console.WriteLine("✓ Token validated successfully");
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"✗ Authentication failed: {context.Exception.Message}");
            Console.WriteLine($"Exception type: {context.Exception.GetType().Name}");
            return Task.CompletedTask;
        }
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
System.Console.WriteLine(mailSenderAddress);
System.Console.WriteLine(mailConnectionString);
System.Console.WriteLine(mailSenderAddress);


if (!string.IsNullOrWhiteSpace(mailConnectionString) && !string.IsNullOrWhiteSpace(mailSenderAddress))
{
    // builder.Services.AddSingleton(sp => new EmailHelper(new EmailClient(mailConnectionString), mailSenderAddress));

    // Register EmailService with proper constructor injection
    builder.Services.AddScoped<IEmailService>(sp =>
    {
        //var helper = sp.GetRequiredService<EmailHelper>();
        return new EmailService(mailSenderAddress);
    });
}
else
{
    builder.Services.AddScoped<IEmailService, NoOpEmailService>();
}


builder.Services.AddScoped<IEmailService, NoOpEmailService>();

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
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddHttpClient<IAgentService, AgentService>();
builder.Services.AddScoped<IBoardService, BoardService>();
builder.Services.AddScoped<IAgentService, AgentService>();
builder.Services.AddScoped<IAiAgentBehaviour, CreataTaskStrategy>();
builder.Services.AddScoped<IAiAgentBehaviour, DefaultAnswerStrategy>();
builder.Services.AddScoped<IAgentStrategyResolver, AgentStrategyResolver>();
builder.Services.AddScoped<ISearchBehaviour, TitleSearchStrategy>();
builder.Services.AddScoped<SearchService>();

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




// ======
//   AI
// ======





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

// var mailConnectionString = Environment.GetEnvironmentVariable("AppSettings_EmailSmtp")
//                              ?? builder.Configuration["Email:ConnectionString"];
// var mailSenderAddress = Environment.GetEnvironmentVariable("AppSettings_EmailFrom")
//                          ?? builder.Configuration["Email:SenderAddress"];

// if (!string.IsNullOrWhiteSpace(mailConnectionString) && !string.IsNullOrWhiteSpace(mailSenderAddress))
// {
//     builder.Services.AddScoped<IEmailService>(sp =>
//         new EmailService(mailConnectionString, mailSenderAddress)
//     );
// }
// else
// {
//     builder.Services.AddScoped<IEmailService, NoOpEmailService>();
// }


app.UseCors("FrontEndUI");

// Developer exception page for dev

app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
