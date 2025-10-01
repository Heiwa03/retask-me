using BusinessLogicLayerCore.Services;
using DataAccessLayerCore.Repositories.Interfaces;
using DataAccessLayerCore.Repositories;
using Moq;
using Xunit;
using BusinessLogicLayerCore.DTOs;

using DataAccessLayerCore.Enum;
using DataAccessLayerCore.Entities;
using Microsoft.AspNetCore.Routing;

namespace BusinessLogicLayerCoreTests.TestProfile;

    public class UserProfileFixture{

        private readonly Mock<IUserRepository> _userRepository = new();
        private readonly ProfileService profileService;

        public UserProfileFixture(){
            profileService = new ProfileService(_userRepository.Object);
        }

        public PostRegisterDTO TestUser => new PostRegisterDTO{
            FirstName = "Lastname",
            LastName = "Firstname",
            Gender = Gender.Male,
        };


        [Fact]
        public async Task GetProfile_SUCCESS()
        {
            // Arrange
            var user = new User
            {
                Uuid = Guid.NewGuid(),
                Username = "Dan",
                NormalizedUsername = "TEST_USER",
                Password = "3cmPeperoni",
                FirstName = TestUser.FirstName,
                LastName = TestUser.LastName,
                Gender = Gender.Male,
                IsVerified = true
            };

            _userRepository
                .Setup(r => r.GetByUuidAsync<User>(user.Uuid))
                .ReturnsAsync(user);

            // Act
            var result = await profileService.GetProfile(user.Uuid);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Lastname", result.FirstName);
            Assert.Equal("Firstname", result.LastName);
            Assert.Equal(Gender.Male, result.Gender);

            _userRepository.Verify(r => r.GetByUuidAsync<User>(user.Uuid), Times.Once);
        }

        [Fact]
        public async Task UpdateProfile_Success(){
            // Arrange
            var user = new User
            {
                Uuid = Guid.NewGuid(),
                Username = "Dan",
                NormalizedUsername = "TEST_USER",
                Password = "3cmPeperoni",
                FirstName = TestUser.FirstName,
                LastName = TestUser.LastName,
                Gender = Gender.Male,
                IsVerified = true
            };
        }
}