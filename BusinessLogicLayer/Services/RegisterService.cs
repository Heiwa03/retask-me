
// System dependency
using System.Security.Cryptography;
using System.Text;

// Used namespaces from BL
using BusinessLogicLayer.Services.Interfaces;
using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.testsBagrin.Entity;
using BusinessLogicLayer.testsBagrin.Interfaces;

// Used namespaces from HL
using HelperLayer.Security;
using HelperLayer.Security.Token;


namespace BusinessLogicLayer.Services{
    public class RegisterService : IRegisterService{
        private readonly IUserRepository _userRepository;

        public RegisterService(IUserRepository _userRepository){
            this._userRepository = _userRepository;
        }

        // 1. Main method for register
        public async Task RegisterUser(RegisterDTO dto){
            // 1,1 TODO Validate Login
            
            // 1.2 Validate rep password
            PasswordHelper.ValidateRegisterData(dto.Password, dto.RepeatPassword);

            // 1.3 TODO Validate email format
            
            // 1.4 HashPassword
            string hashedPassword = PasswordHelper.HashPassword(dto.Password);

            // 1.5 Create user 
            TestUser user = CreateUser(dto, hashedPassword);

            // Save in bd
            await _userRepository.AddUser(user);
        }

        // Create user model
        private TestUser CreateUser(RegisterDTO dto, string hashedPassword){
            string refreshToken = TokenHelper.GenerateRefreshToken();

            TestUser user = new TestUser{
                ID = 0,
                Username = dto.Username,
                HashedPassword = hashedPassword,
                Mail = dto.Mail,
                RefreshToken = refreshToken
            };

            return user;
        }
    }
}