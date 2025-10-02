using BusinessLogicLayerCore.Services;
using DataAccessLayerCore.Repositories.Interfaces;
using Moq;
using Xunit;
using BusinessLogicLayerCore.DTOs;
using DataAccessLayerCore.Enum;
using DataAccessLayerCore.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayerCoreTests.TestProfile
{
    public class UserProfileFixture
    {
        private readonly Mock<IUserRepository> _userRepository = new();
        private readonly ProfileService _profileService;

        public UserProfileFixture()
        {
            _profileService = new ProfileService(_userRepository.Object);
        }

        // ==== Test Data ====
        private static PostRegisterDTO ValidProfile(string first, string last, Gender gender) =>
            new PostRegisterDTO { FirstName = first, LastName = last, Gender = gender };

        private static User CreateUser(Guid uuid, string first = "John", string last = "Marston", Gender gender = Gender.Male) =>
            new User
            {
                Uuid = uuid,
                Username = "Dan",
                NormalizedUsername = "TEST_USER",
                Password = "3cmPeperoni",
                FirstName = first,
                LastName = last,
                Gender = gender,
                IsVerified = true
            };

        private void SetupUserExists(User user) =>
            _userRepository.Setup(r => r.GetByUuidAsync<User>(user.Uuid)).ReturnsAsync(user);

        private void SetupUserNotFound(Guid uuid) =>
            _userRepository.Setup(r => r.GetByUuidAsync<User>(uuid)).ReturnsAsync((User?)null);

        // ==== TESTS ====

        [Fact]
        public async Task RegisterUserProfile_UserExists_UpdatesAndSaves()
        {
            var uuid = Guid.NewGuid();
            var user = CreateUser(uuid);
            SetupUserExists(user);

            await _profileService.RegisterUserProfile(ValidProfile("John", "Marston", Gender.Male), uuid);

            _userRepository.Verify(r => r.Update(It.Is<User>(u =>
                u.FirstName == "John" &&
                u.LastName == "Marston" &&
                u.Gender == Gender.Male
            )), Times.Once);

            _userRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task RegisterUserProfile_UserNotFound_ThrowsException()
        {
            var uuid = Guid.NewGuid();
            SetupUserNotFound(uuid);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _profileService.RegisterUserProfile(new PostRegisterDTO(), uuid));
        }

        [Fact]
        public async Task GetProfile_UserExists_ReturnsDto()
        {
            var uuid = Guid.NewGuid();
            var user = CreateUser(uuid, "Dan", "Marston", Gender.Male);
            SetupUserExists(user);

            var result = await _profileService.GetProfile(uuid);

            Assert.Equal("Dan", result.FirstName);
            Assert.Equal("Marston", result.LastName);
            Assert.Equal(Gender.Male, result.Gender);
        }

        [Fact]
        public async Task GetProfile_UserNotFound_ReturnsEmptyDto()
        {
            var uuid = Guid.NewGuid();
            SetupUserNotFound(uuid);

            var result = await _profileService.GetProfile(uuid);

            Assert.NotNull(result);
            Assert.Null(result.FirstName);
            Assert.Null(result.LastName);
        }

        [Fact]
        public async Task UpdateProfile_UserExists_UpdatesAndSaves()
        {
            var uuid = Guid.NewGuid();
            var user = CreateUser(uuid);
            SetupUserExists(user);

            var dto = ValidProfile("Updated", "User", Gender.Girl);

            await _profileService.UpdateProfile(dto, uuid);

            _userRepository.Verify(r => r.Update(It.Is<User>(u =>
                u.FirstName == "Updated" &&
                u.LastName == "User" &&
                u.Gender == Gender.Girl
            )), Times.Once);

            _userRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateProfile_UserNotFound_ThrowsException()
        {
            var uuid = Guid.NewGuid();
            SetupUserNotFound(uuid);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _profileService.UpdateProfile(new PostRegisterDTO(), uuid));
        }
    }
}
