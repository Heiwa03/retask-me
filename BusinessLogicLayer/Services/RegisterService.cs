
// System dependency
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

// Used namespaces from BL
using BusinessLogicLayer.Services.Interfaces;
using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.testsBagrin.Entity;
using BusinessLogicLayer.testsBagrin.Interfaces;

// Used namespaces from HL
using HelperLayer.Security;
using HelperLayer.Security.Token;


// Used namespaces from DAL
using DataAccessLayer.Repositories.Interfaces;
using DataAccessLayer.Entities;



namespace BusinessLogicLayer.Services{
    public class RegisterService : IRegisterService{
        private readonly IUserRepository _userRepository;
        private readonly IBaseRepository _baseRepository;

        public RegisterService(IUserRepository _userRepository, IBaseRepository _baseRepository){
            this._userRepository = _userRepository;
            this._baseRepository = _baseRepository;
        }

        // 1. Main method for register
        public async Task RegisterUser(RegisterDTO dto){
            // Check if username is unique
            CheckUniqueUsername(dto.Username);

            // Validate rep password
            CheckPasswordRequirements(dto.Password);

            // Validate password strengh
            CheckRepeatPassword(dto.Password, dto.RepeatPassword);
            
            // TODO Validate email format
            
            // Hashing Password with BCrypt
            string hashedPassword = PasswordHelper.HashPassword(dto.Password);

            // Create user 
            User user = CreateUser(dto, hashedPassword);
            _baseRepository.Add(user);
            await _baseRepository.SaveChangesAsync();

            // Create session
            UserSession userSession = CreateSession(user);
            _baseRepository.Add(userSession);
            await SaveChanges();
        }

        // Check if username is unique
        private void CheckUniqueUsername(string Username){
            if(_userRepository.IsUserNameOccupied(Username)){
                throw new InvalidOperationException("Username already exists");
            }
        }

        // Check repeat password requirements
        private void CheckRepeatPassword(string password, string repeatPassword) {
            if(!PasswordHelper.ValidateRegisterData(password, repeatPassword)){
                throw new InvalidOperationException("Password does not match");
            }
        }

        // Check password strengh
        private void CheckPasswordRequirements(string password){
            if(!PasswordHelper.IsPasswordStrong(password)){
                throw new InvalidOperationException("Password is not strong");
            }
        }

        // Create user model
        private User CreateUser(RegisterDTO dto, string hashedPassword){
            User user = new User{
                Id = 0,
                Uuid = Guid.NewGuid(),
                Username = dto.Username,
                NormalizedUsername = dto.Username.ToUpperInvariant(),
                Password = hashedPassword,
                //Mail = dto.Mail,
            };
            return user;
        }

        // Create user session
        private UserSession CreateSession(User user){
            string generatedRefreshToken = TokenHelper.GenerateRefreshToken();

            UserSession userSession = new UserSession{
                Id = 0,
                Uuid = user.Uuid,
                User = user,
                UserId = user.Id,
                RefreshToken = generatedRefreshToken,
                JwtId = user.Uuid.ToString(),
                RefreshTokenExpiration = DateTime.UtcNow.AddDays(7),
                Redeemed = false
            };

            return userSession;
        }

        // Save changes in bd
        private async Task SaveChanges(){
            try{
                await _baseRepository.SaveChangesAsync();
            }
            catch (DbUpdateException ex){
                Console.WriteLine(ex.InnerException?.Message);
                throw; 
            }
        }

    }
}