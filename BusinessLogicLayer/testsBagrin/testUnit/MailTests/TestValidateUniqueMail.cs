using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Services;
using Xunit;

using BusinessLogicLayer.testsBagrin.Repo.Fake;

namespace BusinessLogicLayer.testsBagrin.testUnit.MailTests{
    public class TestValidateUniqueMail{
        // -------------------------
        // VALIDATE Unique login
        // -------------------------
         [Fact]
        public async Task TestNullMail(){
            FakeUserRep repo = new FakeUserRep();
            RegisterService service = new RegisterService(repo);

            RegisterDTO dto = new RegisterDTO{
                Username = "testuser",
                Password = "Password",
                RepeatPassword = "Password",
                Mail = ""
            };

            await service.RegisterUser(dto);
        }

        [Fact]
        public async Task TestFailUniqueMail(){
            FakeUserRep repo = new FakeUserRep();
            RegisterService service = new RegisterService(repo);

            RegisterDTO dto = new RegisterDTO{
                Username = "testuser",
                Password = "Password",
                RepeatPassword = "Password"
            };

            await service.RegisterUser(dto);

            RegisterDTO dto1 = new RegisterDTO{
                Username = "testuser",
                Password = "Password",
                RepeatPassword = "Password"
            };

            var e = await Assert.ThrowsAsync<ArgumentException>(async () => 
            {
                await service.RegisterUser(dto1);       
            });
        }

        [Fact]
        public async Task TestSuccessUniqueMail(){
            FakeUserRep repo = new FakeUserRep();
            RegisterService service = new RegisterService(repo);

            RegisterDTO dto = new RegisterDTO{
                Username = "testuser",
                Password = "Password",
                RepeatPassword = "Password"
            };

            await service.RegisterUser(dto);

            RegisterDTO dto1 = new RegisterDTO{
                Username = "test",
                Password = "Password",
                RepeatPassword = "Password"
            };

            var e = await Assert.ThrowsAsync<ArgumentException>(async () => 
            {
                await service.RegisterUser(dto1);       
            });
        }

    }
}

