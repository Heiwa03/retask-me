
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;

namespace HelperLayer.Security{
    public static class PasswordHelper{
        
        // Validate rep passowrd
        public static bool ValidateRegisterData(string password, string repPassword){
            //return string.Equals(password, repPassword);
            return password == repPassword;
        }

        // Hash password (BCrypt, 12)
        public static string HashPassword(string password){
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

        // Method for checking in auth 
        public static bool VerifyHashedPassword(string password, string hashedPassword){
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        public static bool IsPasswordStrong(string password){
            char[] charPass = password.ToCharArray();

            bool hasUpper = false;
            bool hasLower = false;
            bool hasDigit = false;
            bool hasSpecial = false;

            for (int i = 0; i < charPass.Length; i++){
                if (char.IsUpper(charPass[i])) hasUpper = true;
                else if (char.IsLower(charPass[i])) hasLower = true;
                else if (char.IsDigit(charPass[i])) hasDigit = true;
                else hasSpecial = true;
            }

            return hasUpper && hasLower && hasDigit && hasSpecial;
        }
    }
}