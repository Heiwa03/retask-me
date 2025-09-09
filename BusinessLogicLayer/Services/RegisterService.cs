

using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Cryptography;
using System.Text;


using BusinessLogicLayer.Services.Interfaces;
using BusinessLogicLayer.DTOs;


namespace BusinessLogicLayer.Services{
    public class RegisterService : IRegisterService{
        // 1. Main method for register
        public async Task RegisterUser(RegisterDTO dto){
            // Validate login / password / rep password
            ValidateRegisterData(dto);

            // HashPassword
            HashPassword(dto.Password);

            // Save in bd
            var user = new User{
                Login = dto.Login,
                
            }
        }

        // 2. Validate rep passowrd
        private static void ValidateRegisterData(RegisterDTO dto){
            if(dto.Password != dto.RepeatPassword){
                throw new ArgumentException("Passwords do not match");
            }
        }

        // 3. Hash password

        private static string HashPassword(string password){
            using (SHA256 sha256 = SHA256.Create()){
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                
                return Convert.ToBase64String(hash);
            }
        }
    }
}