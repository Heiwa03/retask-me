using Moq;
using Xunit;

using DataAccessLayerCore.Repositories.Interfaces;
using BusinessLogicLayerCore.Services;
using BusinessLogicLayerCore.DTOs;
using DataAccessLayerCore.Entities;

namespace BusinessLogicLayerCoreTests.TestRegister;

    public class RegisterServiceFixture{
        private readonly Mock<IUserRepository> _userRepository = new();
        private readonly RegisterService registerService;

        public RegisterServiceFixture(){
            registerService = new RegisterService(_userRepository.Object);
        }

        public static class Globals{
            public static string mail = "unique@mail.com";
            public static string weekPassword = "1cm";
            public static string password = "3centimeterS@";
            public static string repeatPassword = "3centimeterS@";
            public static string wrongRepPassword = "3.5centimeters5%";
        }

        

        [Fact]
        public async Task RegisterUser_Success(){
            // Averange
            var dto = new RegisterDTO{
                Mail = Globals.mail,
                Password = Globals.repeatPassword,
                RepeatPassword = Globals.repeatPassword
            };

            _userRepository.Setup(r => r.IsUserNameOccupied(dto.Mail)).Returns(false);

            _userRepository
                .Setup(repo => repo.Add(It.IsAny<User>()));
            _userRepository
                .Setup(repo => repo.Add(It.IsAny<UserSession>()));
            _userRepository
                .Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.FromResult(1));

            // Act
            await registerService.RegisterUser(dto);

            // Assert
            _userRepository.Verify(repo => repo.IsUserNameOccupied(dto.Mail), Times.Once);
            _userRepository.Verify(repo => repo.Add(It.Is<User>(u => u.Username == dto.Mail)), Times.Once);
            _userRepository.Verify(repo => repo.Add(It.Is<UserSession>(s => s.User.Username == dto.Mail)), Times.Once);
            _userRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task RegisterUser_FAIL_MailOccupied(){
            // Averange
            var dto = new RegisterDTO{
                Mail = Globals.mail,
                Password = Globals.repeatPassword,
                RepeatPassword = Globals.repeatPassword
            };

            var dto2 = new RegisterDTO{
                Mail = Globals.mail,
                Password = Globals.repeatPassword,
                RepeatPassword = Globals.repeatPassword
            };

            _userRepository
                .Setup(repo => repo.Add(It.IsAny<User>()));
            _userRepository
                .Setup(repo => repo.Add(It.IsAny<UserSession>()));
            _userRepository
                .Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.FromResult(1));

            // Act
            await registerService.RegisterUser(dto);

            await registerService.RegisterUser(dto2);

            // Assert
            _userRepository.Verify(repo => repo.IsUserNameOccupied(dto.Mail), Times.Once);
            _userRepository.Verify(repo => repo.Add(It.Is<User>(u => u.Username == dto.Mail)), Times.Once);
            _userRepository.Verify(repo => repo.Add(It.Is<UserSession>(s => s.User.Username == dto.Mail)), Times.Once);
            _userRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }


}