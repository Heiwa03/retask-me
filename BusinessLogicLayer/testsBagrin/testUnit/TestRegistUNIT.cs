using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Services;
using Xunit;

using BusinessLogicLayer.testsBagrin.Repo.Fake;

namespace BusinessLogicLayer.testsBagrin{
    public class TestRegistUNIT{
        
        // Validate password success
        [Fact]
        public async Task TestPasswordSuccess(){
            FakeUserRep repo = new FakeUserRep();
            RegisterService service = new RegisterService(repo);

            RegisterDTO dto = new RegisterDTO{
                Login = "testuser",
                Password = "Password",
                RepeatPassword = "Password"
            };

            await service.RegisterUser(dto);

            var users = repo.GetAllUsers();
            Assert.Single(users);
            Assert.Equal("testuser", users.First().Login);
        }


        // Validate password failed
        [Fact]
        public async Task TestPasswordFail(){
            FakeUserRep repo = new FakeUserRep();
            RegisterService service = new RegisterService(repo);

            var dto = new RegisterDTO{
                Login = "testuser",
                Password = "a",
                RepeatPassword = "Password"
            };

            await service.RegisterUser(dto);

            var users = repo.GetAllUsers();
            Assert.Single(users);
            Assert.Equal("testuser", users.First().Login);
        }
    }
}

