using System.Security.Cryptography;

namespace HelperLayer.Security.Token{
    public class TokenHelper{
        // Generate JWT

        // Generate Refresh token
        public static string GenerateRefreshToken(){
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(randomBytes); 
        }
    }
}