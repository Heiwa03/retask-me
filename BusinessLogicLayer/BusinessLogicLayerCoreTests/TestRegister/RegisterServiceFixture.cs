using Moq;
using Xunit;

using DataAccessLayerCore.Repositories.Interfaces;
using BusinessLogicLayerCore.Services;


namespace BusinessLogicLayerCoreTests.TestRegister;

    public class RegisterServiceFixture{
        private readonly Mock<IUserRepository> _userRepository = new();
        private readonly RegisterService registerService;

        public RegisterServiceFixture(){
            registerService = new RegisterService(_userRepository.Object);
        }

        [Fact]
        public void CheckUniqueMail_WhenMailIsUnique_ShouldNotThrow(){
            var mail = "unique@mail.com";
            _userRepository.Setup(r => r.IsUserNameOccupied(mail));

            var exception = Record.Exception(() => registerService.CheckUniqueMail(mail));
            Assert.Null(exception); 
        }

        [Fact]
        public void CheckUniqueMail_WhenMailExists_ShouldThrow(){
            _userRepository
                .Setup(r => r.IsUserNameOccupied("test@mail.com"))
                .Returns(true);

            var ex = Assert.Throws<InvalidOperationException>(() =>
                registerService.CheckUniqueMail("test@mail.com")
            );

            Assert.Equal("Username already exists", ex.Message);
        }
    }