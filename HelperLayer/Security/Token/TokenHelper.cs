using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace HelperLayer.Security.Token
{
    public static class TokenHelper
    {
        /// <summary>
        /// Generate a JWT access token signed with the private key.
        /// </summary>
        /// <param name="userUuid">User UUID</param>
        /// <param name="username">Username</param>
        /// <param name="signingCredentials">SigningCredentials using private RSA key</param>
        /// <param name="issuer">JWT issuer</param>
        /// <param name="audience">JWT audience</param>
        /// <param name="expiresMinutes">Token expiry in minutes</param>
        /// <returns>JWT token string</returns>
        public static string GenerateJwtToken(
            Guid userUuid,
            string username,
            SigningCredentials signingCredentials,
            string issuer,
            string audience,
            int expiresMinutes = 60)
        {
            if (userUuid == Guid.Empty) throw new ArgumentException("User UUID cannot be empty", nameof(userUuid));
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Username cannot be null or empty", nameof(username));
            if (signingCredentials == null) throw new ArgumentNullException(nameof(signingCredentials));
            if (string.IsNullOrWhiteSpace(issuer)) throw new ArgumentNullException(nameof(issuer));
            if (string.IsNullOrWhiteSpace(audience)) throw new ArgumentNullException(nameof(audience));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userUuid.ToString()),   // user identifier
                new Claim(JwtRegisteredClaimNames.UniqueName, username),      // username
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // unique token ID
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expiresMinutes),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Generate a cryptographically secure refresh token.
        /// </summary>
        /// <returns>Base64 string refresh token</returns>
        public static string GenerateRefreshToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(randomBytes);
        }

        /// <summary>
        /// Validate a JWT token and return the user UUID from the "sub" claim.
        /// </summary>
        /// <param name="token">JWT token string</param>
        /// <param name="issuer">Expected issuer</param>
        /// <param name="audience">Expected audience</param>
        /// <param name="publicKey">RSA public key for signature validation</param>
        /// <returns>User UUID as Guid</returns>
        public static Guid ValidateJwtToken(string token, string issuer, string audience, RsaSecurityKey publicKey)
        {
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentNullException(nameof(token));
            if (string.IsNullOrWhiteSpace(issuer)) throw new ArgumentNullException(nameof(issuer));
            if (string.IsNullOrWhiteSpace(audience)) throw new ArgumentNullException(nameof(audience));
            if (publicKey == null) throw new ArgumentNullException(nameof(publicKey));

            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(5),
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = publicKey
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                // Extract user UUID from "sub" claim
                var subClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                if (string.IsNullOrWhiteSpace(subClaim)) throw new SecurityTokenException("Token does not contain sub claim");

                return Guid.Parse(subClaim);
            }
            catch (SecurityTokenExpiredException)
            {
                throw; // token expired
            }
            catch (Exception ex)
            {
                throw new SecurityTokenException("Invalid token", ex);
            }
        }
    }
}
