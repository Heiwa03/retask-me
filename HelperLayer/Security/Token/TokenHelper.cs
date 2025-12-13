using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace HelperLayer.Security.Token{
    public static class TokenHelper{
        // Generate JWT Token using provided credentials; issuer/audience optional

        public static string GenerateJwtToken(Guid userUuid, string username, SigningCredentials signingCredentials, string? issuer, string? audience, int expiresMinutes = 60)
        {
            if (userUuid == Guid.Empty)
                throw new ArgumentException("User UUID cannot be empty", nameof(userUuid));

            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be null or empty", nameof(username));

            if (signingCredentials == null)
                throw new ArgumentNullException(nameof(signingCredentials));

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, userUuid.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, username)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expiresMinutes),
                SigningCredentials = signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
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