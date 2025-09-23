using DataAccessLayerCore;
using DataAccessLayerCore.Entities;
using DataAccessLayerCore.Repositories;
using DataAccessLayerCoreTests.MockDbSet;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Xunit;

namespace DataAccessLayerCoreTests
{
    public class UserSessionRepositoryTests
    {
        private readonly Mock<DatabaseContext> _mockContext;

        private readonly UserSessionRepository _userSessionRepository;

        public UserSessionRepositoryTests()
        {
            var mockOptions = new DbContextOptions<DatabaseContext>();
            _mockContext = new Mock<DatabaseContext>(mockOptions);

            _userSessionRepository = new UserSessionRepository(_mockContext.Object);
        }

        private static User GetSingleUserEntity()
        {
            return new User
            {
                Id = 1,
                Uuid = Guid.NewGuid(),
                isActive = true,
                CreatedDate = DateTime.Now,
                Username = "Penis@gmail.com",
                NormalizedUsername = "PENIS@GMAIL.COM",
                Password = "sdasdasdaawdasdasd"
            };
        }

        [Fact]
        public async Task GetSessionByRefreshTokenAsync_ReturnsValidSession_WhenRefreshTokenExists()
        {
            // Arrange
            var refreshToken = Guid.NewGuid().ToString();
            var user = GetSingleUserEntity();
            var sessions = new List<UserSession>
            {
                new UserSession
                {
                    Id = 1,
                    Uuid = Guid.NewGuid(),
                    isActive = true,
                    CreatedDate = DateTime.Now,
                    UserId = user.Id,
                    User = user,
                    RefreshToken = refreshToken,
                    JwtId = Guid.NewGuid().ToString(),
                    RefreshTokenExpiration = DateTime.Now.AddDays(7),
                    Redeemed = false
                }
            };

            var mockDbSet = sessions.AsDbSetMock();

            _mockContext.Setup(c => c.Set<UserSession>()).Returns(mockDbSet.Object);

            var repository = new UserSessionRepository(_mockContext.Object);

            // Act
            var result = await repository.GetSessionByRefreshTokenAsync(refreshToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(refreshToken, result.RefreshToken);
            Assert.Equal(user.Id, result.UserId);
        }

        [Fact]
        public async Task GetSessionByRefreshTokenAsync_ReturnsNull_WhenRefreshTokenDoesntExist()
        {
            // Arrange
            var refreshToken = Guid.NewGuid().ToString();
            var user = GetSingleUserEntity();
            var sessions = new List<UserSession>
            {
                new UserSession
                {
                    Id = 1,
                    Uuid = Guid.NewGuid(),
                    isActive = true,
                    CreatedDate = DateTime.Now,
                    UserId = user.Id,
                    User = user,
                    RefreshToken = Guid.NewGuid().ToString(),
                    JwtId = Guid.NewGuid().ToString(),
                    RefreshTokenExpiration = DateTime.Now.AddDays(7),
                    Redeemed = false
                }
            };

            var mockDbSet = sessions.AsDbSetMock();

            _mockContext.Setup(c => c.Set<UserSession>()).Returns(mockDbSet.Object);

            var repository = new UserSessionRepository(_mockContext.Object);

            // Act
            var result = await repository.GetSessionByRefreshTokenAsync(refreshToken);

            // Assert
            Assert.Null(result);
        }
    }
}