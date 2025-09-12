
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;

namespace HelperLayer.Security{
    public static class PasswordHelper{
        
        // Validate rep passowrd
        public static void ValidateRegisterData(string password, string repPassword){
            if(password != repPassword){
                throw new ArgumentException("Passwords does not match");
            }
        }

        // Hash password (BCrypt, 12)
        public static string HashPassword(string password){
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

        // Method for checking in auth 
        public static bool VerifyHashedPassword(string password, string hashedPassword){
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}