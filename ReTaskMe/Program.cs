using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BusinessLogicLayer.Services;
using BusinessLogicLayer.Services.Interfaces;

using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

using DataAccessLayer.Repositories.Interfaces;
using DataAccessLayer.Repositories;
using DataAccessLayer;
using BusinessLogicLayer.testsBagrin.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("MariaDbConnection"),
        new MySqlServerVersion(new Version(10, 11, 2)) // укажи свою версию MariaDB
    )
);

builder.Configuration.AddUserSecrets<Program>();
var secretKey = builder.Configuration["JwtSecret"];

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// The Scoped lifetime means a new instance is created for each HTTP request.
builder.Services.AddScoped<IAuthService, AuthService>();

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
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        //ValidIssuer = "your-api",
        //ValidAudience = "your-app",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this-is-a-very-secret-and-long-key-for-auth"))
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
