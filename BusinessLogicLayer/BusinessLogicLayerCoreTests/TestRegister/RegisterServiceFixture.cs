using Moq;
using Xunit;

using DataAccessLayerCore.Repositories.Interfaces;
using BusinessLogicLayerCore.Services;
using System.Net.Mail;


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


        // MAIL
        [Fact]
        public void CheckUniqueMail_WhenMailIsUnique_ShouldNotThrow(){
            _userRepository.Setup(r => r.IsUserNameOccupied(Globals.mail));

            var e = Record.Exception(() => registerService.CheckUniqueMail(Globals.mail));
            Assert.Null(e); 
        }

        [Fact]
        public void CheckUniqueMail_WhenMailExists_ShouldThrow(){
            _userRepository
                .Setup(r => r.IsUserNameOccupied(Globals.mail))
                .Returns(true);

            var e = Assert.Throws<InvalidOperationException>(() =>
                registerService.CheckUniqueMail(Globals.mail)
            );

            Assert.Equal("Username already exists", e.Message);
        }

        // REP Passowrd
        [Fact]
        public void CheckRepeatPassword_Succed(){
            _userRepository.Setup(r => r.IsUserNameOccupied(Globals.mail));

            var e = Record.Exception(() => registerService.CheckRepeatPassword(Globals.password, Globals.repeatPassword));

            Assert.Null(e);
        }

        [Fact]
        public void CheckRepeatPassword_Fail(){
            var e = Record.Exception(() => registerService.CheckRepeatPassword(Globals.password, Globals.wrongRepPassword));

            Assert.Equal("Password does not match", e.Message);
        }

        // Strong password
        [Fact]
        public void CheckStrongPassword_Succed(){
            var e = Record.Exception(() => registerService.CheckPasswordRequirements(Globals.password));

            Assert.Null(e);
        }

        [Fact]
        public void CheckStrongPassword_Fail(){
            var e = Record.Exception(() => registerService.CheckPasswordRequirements(Globals.weekPassword));

            Assert.Equal("Password is not strong", e.Message);
        }

        
    }