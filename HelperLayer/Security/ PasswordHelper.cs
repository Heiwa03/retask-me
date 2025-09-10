
using System.Security.Cryptography;
using System.Text;

namespace HelperLayer.Security{
    public static class PasswordHelper{
        // Validate rep passowrd
        public static void ValidateRegisterData(string password, string repPassword){
            if(password != repPassword){
                throw new ArgumentException("Passwords do not match");
            }
        }

        // Hash password (SHA256, no salt)
        public static string HashPassword(string password){
            using (SHA256 sha256 = SHA256.Create()){
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes); // !!!
                
                return Convert.ToBase64String(hash);
            }
        }
    }
}