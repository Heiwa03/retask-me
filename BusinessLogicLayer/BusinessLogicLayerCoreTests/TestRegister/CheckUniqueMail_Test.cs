// using Xunit;
// using DataAccessLayer;
// using BusinessLogicLayer.Services;
// using BusinessLogicLayer.Services.Interfaces;
// using Microsoft.EntityFrameworkCore;
// using System.Threading.Tasks;

namespace BusinessLogicLayerCoreTests.TestRegister{
    public class CheckUniqueMail_Test{
        // private readonly DatabaseContext _context;
        // private readonly RegisterService _registerService;

        // public CheckUniqueMail_Test()
        // {
        //     var options = new DbContextOptionsBuilder<DatabaseContext>()
        //         .UseInMemoryDatabase(databaseName: "TestDb")
        //         .Options;

        //     _context = new DatabaseContext(options);

        //     var baseRepo = new BaseRepository(_context);
        //     var userRepo = new UserRepository(_context, baseRepo);

        //     _registerService = new RegisterService(userRepo, baseRepo);
        // }

        // [Fact]
        // public async Task CheckUniqueMail_Success()
        // {
        //     var dto = new BusinessLogicLayer.DTOs.RegisterDTO
        //     {
        //         Mail = "unique@mail.com",
        //         Password = "Password123@",
        //         RepeatPassword = "Password123@"
        //     };

        //     // Act
        //     var result = await _registerService.RegisterUser(dto);

        //     // Assert
        //     Assert.NotNull(result); 
        // }

        // [Fact]
        // public async Task CheckUniqueMail_Fail()
        // {
        //     var dto1 = new BusinessLogicLayer.DTOs.RegisterDTO
        //     {
        //         Mail = "duplicate@mail.com",
        //         Password = "Password123@",
        //         RepeatPassword = "Password123@"
        //     };

        //     var dto2 = new BusinessLogicLayer.DTOs.RegisterDTO
        //     {
        //         Mail = "duplicate@mail.com",
        //         Password = "Password456@",
        //         RepeatPassword = "Password456@"
        //     };

        //     await _registerService.RegisterUser(dto1);
        //     var ex = await Assert.ThrowsAsync<ApplicationException>(() => _registerService.RegisterUser(dto2));

        //     Assert.Contains("already exists", ex.Message);
        // }
    }
}
