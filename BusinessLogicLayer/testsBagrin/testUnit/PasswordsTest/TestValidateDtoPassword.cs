using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Services;
using Xunit;

using BusinessLogicLayer.testsBagrin.Repo.Fake;

namespace BusinessLogicLayer.testsBagrin.testUnit.PasswordsTest{
    public class TestValidateDtoPassword{
       
        // ---------------------
        // VALIDATE DTO PASSWORD
        // ---------------------

        // Validate DTO password success
        [Fact]
        public async Task TestPasswordSuccess(){
            FakeUserRep repo = new FakeUserRep();
            RegisterService service = new RegisterService(repo);

            RegisterDTO dto = new RegisterDTO{
                Username = "testuser",
                Password = "Password",
                RepeatPassword = "Password"
            };

            await service.RegisterUser(dto);

        }


        // Validate DTO password failed
        [Fact]
        public async Task TestPasswordFail(){
            FakeUserRep repo = new FakeUserRep();
            RegisterService service = new RegisterService(repo);

            RegisterDTO dto = new RegisterDTO{
                Username = "testuser",
                Password = "password",
                RepeatPassword = "Password"
            };

            await service.RegisterUser(dto);

        }
    }
}

