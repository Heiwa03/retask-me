using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace HelperLayer.Security.Token{
    public static class TokenHelper{
        // Generate JWT Token using provided credentials; issuer/audience optional
        public static string GenerateJwtToken(string username, SigningCredentials signingCredentials, string? issuer, string? audience, int expiresMinutes = 60){
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Generate Refresh token
        public static string GenerateRefreshToken(){
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(randomBytes); 
        }
        public static string ValidateJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            // Optional: validate expiry
            if (jwtToken.ValidTo < DateTime.UtcNow)
                throw new SecurityTokenExpiredException();

            var email = jwtToken.Subject; // "sub" claim
            if (string.IsNullOrEmpty(email))
                throw new SecurityTokenException("Invalid token");

            return email;
        }
    }
}