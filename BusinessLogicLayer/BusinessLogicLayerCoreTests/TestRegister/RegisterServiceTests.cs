using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLogicLayerCore.DTOs;
using BusinessLogicLayerCore.Services;
using BusinessLogicLayerCore.Services.Interfaces;
using DataAccessLayerCore.Entities;
using DataAccessLayerCore.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using FluentAssertions;
using HelperLayer.Security;
using Azure.Communication.Email;

namespace BusinessLogicLayerCoreTests.TestRegister
{
    public class RegisterServiceTests
    {
        private readonly Mock<IUserRepository> _userRepository = new();
        private readonly Mock<IEmailService> _emailService = new();
        private readonly Mock<IConfiguration> _configuration = new();

        private RegisterService CreateService()
        {
            _configuration.Setup(c => c["Frontend:BaseUrl"]).Returns("http://localhost:4200");

            // We don't care about email or JWT for these tests
            _emailService.Setup(e => e.SendEmailAsync(It.IsAny<List<string>>(),
                                                      It.IsAny<string>(),
                                                      It.IsAny<string>()))
                         .ReturnsAsync(true);

            // Use a dummy SigningCredentials with live RSA (not disposed)
            var dummyRsa = System.Security.Cryptography.RSA.Create();
            var dummyCreds = new Microsoft.IdentityModel.Tokens.SigningCredentials(
                new Microsoft.IdentityModel.Tokens.RsaSecurityKey(dummyRsa),
                Microsoft.IdentityModel.Tokens.SecurityAlgorithms.RsaSha256
            );

            var dummyEmailHelper = new EmailHelper(null, "test@mail.ru");


            return new RegisterService(
                _userRepository.Object,
                _emailService.Object,
                dummyEmailHelper,
                dummyCreds,
                _configuration.Object
            );
        }

        [Fact]
        public async Task RegisterUser_Throws_WhenEmailTaken()
        {
            var dto = new RegisterDTO
            {
                Mail = "taken@site.com",
                Password = "Aa1!aaaa",
                RepeatPassword = "Aa1!aaaa"
            };

            _userRepository.Setup(r => r.IsUsernameOccupied(dto.Mail)).Returns(true);

            var service = CreateService();

            Func<Task> act = async () => await service.RegisterUser(dto);

            await act.Should().ThrowAsync<InvalidOperationException>()
                     .WithMessage("Email already exists");
        }

        [Fact]
        public async Task RegisterUser_Throws_WhenPasswordMismatch()
        {
            var dto = new RegisterDTO
            {
                Mail = "ok@site.com",
                Password = "Aa1!aaaa",
                RepeatPassword = "WrongPass1!"
            };

            _userRepository.Setup(r => r.IsUsernameOccupied(dto.Mail)).Returns(false);

            var service = CreateService();

            Func<Task> act = async () => await service.RegisterUser(dto);

            await act.Should().ThrowAsync<InvalidOperationException>()
                     .WithMessage("Passwords do not match");
        }

        [Fact]
        public async Task RegisterUser_Throws_WhenPasswordWeak()
        {
            var dto = new RegisterDTO
            {
                Mail = "ok@site.com",
                Password = "weakpass",
                RepeatPassword = "weakpass"
            };

            _userRepository.Setup(r => r.IsUsernameOccupied(dto.Mail)).Returns(false);

            var service = CreateService();

            Func<Task> act = async () => await service.RegisterUser(dto);

            await act.Should().ThrowAsync<InvalidOperationException>()
                     .WithMessage("Password is not strong enough");
        }

        [Fact]
        public async Task RegisterUser_Succeeds_WithValidData()
        {
            var dto = new RegisterDTO
            {
                Mail = "ok@site.com",
                Password = "Aa1!aaaa",
                RepeatPassword = "Aa1!aaaa"
            };

            _userRepository.Setup(r => r.IsUsernameOccupied(dto.Mail)).Returns(false);
            _userRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            var service = CreateService();

            await service.RegisterUser(dto);

            _userRepository.Verify(r => r.Add(It.Is<User>(u => u.NormalizedUsername == "OK@SITE.COM")), Times.Once);
            _userRepository.Verify(r => r.Add(It.Is<UserSession>(s => s.User != null)), Times.Once);
        }
    }
}