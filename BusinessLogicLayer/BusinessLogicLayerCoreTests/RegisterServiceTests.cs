using System;
using System.Threading.Tasks;
using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Services;
using BusinessLogicLayer.Services.Interfaces;
using DataAccessLayer.Entities;
using DataAccessLayer.Repositories.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace BusinessLogicLayerCoreTests
{
    public class RegisterServiceTests
    {
        private readonly Mock<IUserRepository> _userRepository = new();
        private readonly Mock<IBaseRepository> _baseRepository = new();

        private RegisterService CreateService()
        {
            return new RegisterService(_userRepository.Object, _baseRepository.Object);
        }

        [Fact]
        public async Task RegisterUser_Throws_WhenEmailTaken()
        {
            // Arrange
            var dto = new RegisterDTO { Mail = "taken@site.com", Password = "Aa1!aaaa", RepeatPassword = "Aa1!aaaa" };
            _userRepository.Setup(r => r.IsEmailOccupied("taken@site.com")).Returns(true);
            var service = CreateService();

            // Act
            var act = async () => await service.RegisterUser(dto);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task RegisterUser_Succeeds_CreatesUserAndSession()
        {
            // Arrange
            var dto = new RegisterDTO { Mail = "ok@site.com", Password = "Aa1!aaaa", RepeatPassword = "Aa1!aaaa" };
            _userRepository.Setup(r => r.IsEmailOccupied("ok@site.com")).Returns(false);
            _baseRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
            var service = CreateService();

            // Act
            await service.RegisterUser(dto);

            // Assert
            _baseRepository.Verify(r => r.Add(It.Is<User>(u => u.NormalizedUsername == "OK@SITE.COM")), Times.Once);
            _baseRepository.Verify(r => r.Add(It.Is<UserSession>(s => s.User != null)), Times.Once);
            _baseRepository.Verify(r => r.SaveChangesAsync(), Times.AtLeastOnce);
        }
    }
}
