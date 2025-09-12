
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
            CheckUniqueUsername(dto.Username);

            // 1.2 Validate rep password
            PasswordHelper.ValidateRegisterData(dto.Password, dto.RepeatPassword);

            // 1.3 TODO Validate email format
            CheckUniqueMail(dto.Mail);
            
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

        // Check unique username
        private void CheckUniqueUsername(string Username){
            var allUsers = _userRepository.GetAllUsers();

            foreach(var user in allUsers){
                if(string.Equals(user.Username, Username)){
                    throw new ArgumentException("Username already exist");
                }
            }
        }

        // Check unique mail
        private void CheckUniqueMail(string? Mail){
            var allUsers = _userRepository.GetAllMails();

            foreach(var user in allUsers){
                if(string.Equals(user.Mail, Mail)){
                    throw new ArgumentException("This Mail already exist");
                }
            }
        }
    }
}