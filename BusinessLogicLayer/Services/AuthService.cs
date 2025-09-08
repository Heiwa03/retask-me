using BusinessLogicLayer.Services;
using BusinessLogicLayer.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
namespace BusinessLogicLayer.Services
{
    public class AuthService : IAuthService
    {
        private readonly ILoginChecker _loginChecker;

        // JWT Signing key
        private readonly string _jwtSecretKey;

        public AuthService(ILoginChecker loginChecker, IConfiguration configuration)
        {
            _loginChecker = loginChecker;
            _jwtSecretKey = configuration["JwtSecret"];

            if (string.IsNullOrEmpty(_jwtSecretKey))
            {
                throw new ApplicationException("JWT secret key is not configured");
            }
        }

        public Task<AuthResponse> LoginAsync(string username, string password)
        {
            if (_loginChecker.CheckCredentials(username, password))
            {
                var token = GenerateJwtToken(username);
                return Task.FromResult(new AuthResponse { Token = token });
            }

            return Task.FromResult<AuthResponse>(null);
        }

        private string GenerateJwtToken(string username)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Use the secret key to create signing credentials.
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create the token 
            var token = new JwtSecurityToken(
                //issuer: "your-api",
                //audience: "your-app",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30), // Token expires in 30 minutes.
                signingCredentials: credentials);

            // The handler writes the token into a string.
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
