// using Moq;
// using Xunit;

// using DataAccessLayerCore.Repositories.Interfaces;
// using BusinessLogicLayerCore.Services;
// using BusinessLogicLayerCore.DTOs;
// using DataAccessLayerCore.Entities;

// using HelperLayer.Security;
// using Microsoft.IdentityModel.Tokens;
// using Castle.Core.Configuration;


// namespace BusinessLogicLayerCoreTests.TestRegister;

//     public class RegisterServiceFixture
//     {
//         private readonly Mock<IUserRepository> _userRepository = new();
//         private readonly Mock<IConfiguration> _configuration = new();
//         private readonly EmailHelper _emailHelper;
//         private readonly SigningCredentials _signingCredentials;

//         public RegisterService RegisterService { get; }

//         public RegisterServiceFixture()
//         {
//             // Настройка IConfiguration
//             _configuration.Setup(c => c["Frontend:BaseUrl"]).Returns("http://localhost:5000");

//             _emailHelper = new EmailHelper(); // Если нужен мок, можно сделать Mock<EmailHelper>
//             var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("SuperSecretTestKey123!")); 
//             _signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//             // Создание тестируемого сервиса
//             RegisterService = new RegisterService(
//                 _userRepository.Object,
//                 _emailHelper,
//                 _signingCredentials,
//                 _configuration.Object
//             );
//         }
    

//         public static class Globals{
//             public static string mail = "unique@mail.com";
//             public static string weekPassword = "1cm";
//             public static string password = "3centimeterS@";
//             public static string repeatPassword = "3centimeterS@";
//             public static string wrongRepPassword = "3.5centimeters5%";
//         }


       


// }