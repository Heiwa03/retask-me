/*using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using BusinessLogicLayerCore.Services;
using BusinessLogicLayerCore.Services.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;
using DataAccessLayerCore.Repositories.Interfaces;
using DataAccessLayerCore.Entities;

namespace BusinessLogicLayerCoreTests
{
    public class AuthServiceTests
    {
        private static SigningCredentials CreateTestSigningCredentials()
        {
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("supersecret_supersecret_supersecret_123"));
            return new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        }

        private AuthService CreateService(ILoginChecker loginChecker, IConfiguration config)
        {
            var userRepo = new Mock<IUserRepository>();
            var sessionRepo = new Mock<IUserSessionRepository>();
            return new AuthService(loginChecker, userRepo.Object, sessionRepo.Object, config, CreateTestSigningCredentials());
        }

        [Fact]
        public async Task LoginAsync_ReturnsToken_OnValidCredentials()
        {
            // Arrange
            var checker = new Mock<ILoginChecker>();
            checker.Setup(c => c.CheckCredentials("a@b.com", "P@ssw0rd")).Returns(true);
            var inMemory = new ConfigurationBuilder().AddInMemoryCollection(new[]
            {
                new KeyValuePair<string,string?>("Authorization:Issuer","ReTaskMe"),
                new KeyValuePair<string,string?>("Authorization:Audience","ReTaskMe.Controllers")
            }).Build();

            // User repository setup
            var userRepo = new Mock<IUserRepository>();
            userRepo.Setup(r => r.GetUserByUsername("a@b.com"))
                .ReturnsAsync(new User
                {
                    Id = 1,
                    Uuid = Guid.NewGuid(),
                    Username = "a@b.com",
                    NormalizedUsername = "A@B.COM",
                    Password = "hashed",
                    IsVerified = true
                });

            var sessionRepo = new Mock<IUserSessionRepository>();
            sessionRepo.Setup(r => r.RemoveSessionByUserIdAsync(It.IsAny<long>()))
                .Returns(Task.CompletedTask);
            sessionRepo.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            var service = new AuthService(checker.Object, userRepo.Object, sessionRepo.Object, inMemory, CreateTestSigningCredentials());

            // Act
            var result = await service.LoginAsync("a@b.com", "P@ssw0rd");

            // Assert
            result.Should().NotBeNull();
            result!.Token.Should().NotBeNullOrEmpty();
            result.RefreshToken.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task LoginAsync_IssuesToken_WithExpectedClaims_And_Validates()
        {
            // Arrange
            var email = "user@test.local";
            var checker = new Mock<ILoginChecker>();
            checker.Setup(c => c.CheckCredentials(email, "P@ssw0rd")).Returns(true);
            var issuer = "ReTaskMe";
            var audience = "ReTaskMe.Controllers";
            var config = new ConfigurationBuilder().AddInMemoryCollection(new[]
            {
                new KeyValuePair<string,string?>("Authorization:Issuer", issuer),
                new KeyValuePair<string,string?>("Authorization:Audience", audience)
            }).Build();

            var signingCredentials = CreateTestSigningCredentials();

            var userRepo = new Mock<IUserRepository>();
            userRepo.Setup(r => r.GetUserByUsername(email))
                .ReturnsAsync(new User
                {
                    Id = 2,
                    Uuid = Guid.NewGuid(),
                    Username = email,
                    NormalizedUsername = email.ToUpperInvariant(),
                    Password = "hashed",
                    IsVerified = true
                });

            var sessionRepo = new Mock<IUserSessionRepository>();
            sessionRepo.Setup(r => r.RemoveSessionByUserIdAsync(It.IsAny<long>()))
                .Returns(Task.CompletedTask);
            sessionRepo.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            var service = new AuthService(checker.Object, userRepo.Object, sessionRepo.Object, config, signingCredentials);

            // Act
            var login = await service.LoginAsync(email, "P@ssw0rd");
            login.Should().NotBeNull();

            // Parse
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(login!.Token);

            // Claims exist
            token.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == email);

            // Validate signature and issuer/audience
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = (SymmetricSecurityKey)signingCredentials.Key,
                ClockSkew = System.TimeSpan.FromSeconds(1)
            };

            handler.ValidateToken(login.Token, validationParameters, out var validatedToken);
            validatedToken.Should().NotBeNull();
        }

        [Fact]
        public async Task LoginAsync_ReturnsNull_OnInvalidCredentials()
        {
            // Arrange
            var checker = new Mock<ILoginChecker>();
            checker.Setup(c => c.CheckCredentials("a@b.com", "bad")).Returns(false);
            var inMemory = new ConfigurationBuilder().AddInMemoryCollection().Build();
            var userRepo = new Mock<IUserRepository>();
            var sessionRepo = new Mock<IUserSessionRepository>();
            var service = new AuthService(checker.Object, userRepo.Object, sessionRepo.Object, inMemory, CreateTestSigningCredentials());

            // Act
            var result = await service.LoginAsync("a@b.com", "bad");

            // Assert
            result.Should().BeNull();
        }
    }
}

