using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Services;
using Xunit;

using BusinessLogicLayer.testsBagrin.Repo.Fake;

namespace BusinessLogicLayer.testsBagrin{
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

            var users = repo.GetAllUsers();
            Assert.Single(users);
            Assert.Equal("testuser", users.First().Username);
        }


        // Validate DTO password failed
        [Fact]
        public async Task TestPasswordFail(){
            FakeUserRep repo = new FakeUserRep();
            RegisterService service = new RegisterService(repo);

            RegisterDTO dto = new RegisterDTO{
                Username = "testuser",
                Password = "a",
                RepeatPassword = "Password"
            };

            await service.RegisterUser(dto);

            var users = repo.GetAllUsers();
            Assert.Single(users);
            Assert.Equal("testuser", users.First().Username);
        }
    }
}

