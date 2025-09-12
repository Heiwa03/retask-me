using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Services;
using Xunit;

using BusinessLogicLayer.testsBagrin.Repo.Fake;
using BusinessLogicLayer.testsBagrin.Entity;

namespace BusinessLogicLayer.testsBagrin.testUnit.UserCreation{
    public class TestUserCreation{
        [Fact]
        public async Task TestDateCreation(){
            FakeUserRep repo = new FakeUserRep();
            RegisterService service = new RegisterService(repo);

            RegisterDTO dto = new RegisterDTO{
                Username = "testuser",
                Password = "Password123",
                RepeatPassword = "Password123",
                Mail = "test@example.com"
            };

            await service.RegisterUser(dto);

            repo.Print();
        }
    }
}