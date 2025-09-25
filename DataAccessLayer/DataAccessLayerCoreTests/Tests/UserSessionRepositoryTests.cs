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

        [Fact]
        public async Task GetSessionByUserIdAsync_ReturnsSession_WhenSessionExists()
        {
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

            var result = await repository.GetSessionByUserIdAsync(user.Id);

            Assert.NotNull(result);
            Assert.Equal(user.Id, result.UserId);
        }

        [Fact]
        public async Task GetSessionByUserIdAsync_ReturnsNull_WhenSessionInvalid()
        {
            var user = GetSingleUserEntity();
            var sessions = new List<UserSession> { };

            var mockDbSet = sessions.AsDbSetMock();

            _mockContext.Setup(c => c.Set<UserSession>()).Returns(mockDbSet.Object);

            var repository = new UserSessionRepository(_mockContext.Object);

            var result = await repository.GetSessionByUserIdAsync(user.Id);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetSessionByJwtIdAsync_ReturnsSession_WhenSessionExists()
        {
            var jwtId = Guid.NewGuid().ToString();
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
                    JwtId = jwtId,
                    RefreshTokenExpiration = DateTime.Now.AddDays(7),
                    Redeemed = false
                }
            };

            var mockDbSet = sessions.AsDbSetMock();

            _mockContext.Setup(c => c.Set<UserSession>()).Returns(mockDbSet.Object);

            var repository = new UserSessionRepository(_mockContext.Object);

            var result = await repository.GetSessionByJwtIdAsync(jwtId);

            Assert.NotNull(result);
            Assert.Equal(user.Id, result.UserId);
            Assert.Equal(jwtId, result.JwtId);
        }

        [Fact]
        public async Task GetSessionByJwtIdAsync_ReturnsNull_WhenSessionInvalid()
        {
            var jwtId = Guid.NewGuid().ToString();
            var sessions = new List<UserSession> { };

            var mockDbSet = sessions.AsDbSetMock();

            _mockContext.Setup(c => c.Set<UserSession>()).Returns(mockDbSet.Object);

            var repository = new UserSessionRepository(_mockContext.Object);

            var result = await repository.GetSessionByJwtIdAsync(jwtId);

            Assert.Null(result);
        }

        [Fact]
        public async Task RedeemRefreshTokenAsync_SetSessionAsRedeemed_WhenSessionExists()
        {
            var refreshToken = Guid.NewGuid().ToString();
            var jwtId = Guid.NewGuid().ToString();
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

            var resultOfMethod = await repository.RedeemRefreshTokenAsync(refreshToken, jwtId);

            var result = await repository.GetSessionByJwtIdAsync(jwtId);

            Assert.NotNull(result);
            Assert.True(resultOfMethod);
            Assert.Equal(user.Id, result.UserId);
            Assert.Equal(jwtId, result.JwtId);
            Assert.True(result.Redeemed);
        }

        [Fact]
        public async Task RedeemRefreshTokenAsync_ReturnsFalse_WhenSessionDNE()
        {
            var refreshToken = Guid.NewGuid().ToString();
            var jwtId = Guid.NewGuid().ToString();
            var sessions = new List<UserSession> { };

            var mockDbSet = sessions.AsDbSetMock();

            _mockContext.Setup(c => c.Set<UserSession>()).Returns(mockDbSet.Object);

            var repository = new UserSessionRepository(_mockContext.Object);

            Assert.False(await repository.RedeemRefreshTokenAsync(refreshToken, jwtId));
        }
        /* // As it is right now this test can't be run due to the MockDbSet helpers not working right.
        [Fact]
        public async Task RemoveSessionByUserIdAsync_SoftDeletesSessions_WhenSessionsExist()
        {
            // Arrange
            var jwtId1 = Guid.NewGuid().ToString();
            var jwtId2 = Guid.NewGuid().ToString();
            var jwtId3 = Guid.NewGuid().ToString();
            var user = new User
            {
                Id = 1,
                Uuid = Guid.NewGuid(),
                isActive = true,
                CreatedDate = DateTime.Now,
                Username = "user1@test.com",
                NormalizedUsername = "USER1@TEST.COM",
                Password = "password"
            };

            var secondUser = new User
            {
                Id = 2,
                Uuid = Guid.NewGuid(),
                isActive = true,
                CreatedDate = DateTime.Now,
                Username = "user2@test.com",
                NormalizedUsername = "USER2@TEST.COM",
                Password = "password"
            };

            var sessions = new List<UserSession>
            {
                // Sessions to be "soft deleted"
                new UserSession
                {
                    Id = 1,
                    Uuid = Guid.NewGuid(),
                    isActive = true,
                    CreatedDate = DateTime.Now,
                    UserId = user.Id,
                    User = user,
                    RefreshToken = Guid.NewGuid().ToString(),
                    JwtId = jwtId1,
                    RefreshTokenExpiration = DateTime.Now.AddDays(7),
                    Redeemed = false
                },
                new UserSession
                {
                    Id = 2,
                    Uuid = Guid.NewGuid(),
                    isActive = true,
                    CreatedDate = DateTime.Now,
                    UserId = user.Id,
                    User = user,
                    RefreshToken = Guid.NewGuid().ToString(),
                    JwtId = jwtId2,
                    RefreshTokenExpiration = DateTime.Now.AddDays(7),
                    Redeemed = false
                },
                // A session that should not be affected
                new UserSession
                {
                    Id = 3,
                    Uuid = Guid.NewGuid(),
                    isActive = true,
                    CreatedDate = DateTime.Now,
                    UserId = secondUser.Id,
                    User = secondUser,
                    RefreshToken = Guid.NewGuid().ToString(),
                    JwtId = jwtId3,
                    RefreshTokenExpiration = DateTime.Now.AddDays(7),
                    Redeemed = false
                },
            };

            var mockDbSet = sessions.AsDbSetMock();

            _mockContext.Setup(c => c.Set<UserSession>()).Returns(mockDbSet.Object);

            await _userSessionRepository.RemoveSessionByUserIdAsync(user.Id);

            var result1 = await _userSessionRepository.GetSessionByJwtIdAsync(jwtId1);
            var result2 = await _userSessionRepository.GetSessionByJwtIdAsync(jwtId2);
            var result3 = await _userSessionRepository.GetSessionByJwtIdAsync(jwtId3);

            Assert.False(result1.isActive);
            Assert.False(result2.isActive);
            Assert.True(result3.isActive);
        } */  
    }
}