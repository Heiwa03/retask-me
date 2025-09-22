using BusinessLogicLayerCore.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using DataAccessLayerCore.Repositories.Interfaces;
using DataAccessLayerCore.Entities;

namespace BusinessLogicLayerCore.Services
{
    public class AuthService(IUserRepository userRepository, ILoginChecker loginChecker,IConfiguration configuration, SigningCredentials signingCredentials) : IAuthService
    {
        private readonly ILoginChecker _loginChecker;

        // JWT Signing key
    
        private readonly string _jwtSecretKey;
        private readonly SigningCredentials _signingCredentials;
        private readonly string? _issuer;
        private readonly string? _audience;
        private static readonly Dictionary<string, string> _refreshTokenToUsername = new();

        // public AuthService(){
        //     _loginChecker = loginChecker;
        //     _jwtSecretKey = configuration["JwtSecret"];
        //     _signingCredentials = signingCredentials;
        //     _issuer = configuration["Authorization:Issuer"];
        //     _audience = configuration["Authorization:Audience"];

        //     if (string.IsNullOrEmpty(_jwtSecretKey))
        //     {
        //         throw new ApplicationException("JWT secret key is not configured");
        //     }
        // }

        public async Task<AuthResponse> LoginAsync(string username, string password) // THIS
        {
            var existsUser = await userRepository.GetUserByUsername(username) ?? throw new Exception("Someting went wrong."); // THIS

            if (_loginChecker.CheckCredentials(username, password))
            {
                var token = GenerateJwtToken(existsUser.Uuid);
                var refreshToken = GenerateRefreshToken();
                _refreshTokenToUsername[refreshToken] = username;
                return new AuthResponse { Token = token, RefreshToken = refreshToken };
            }

            return new AuthResponse();
        }

        public Task<AuthResponse> RefreshAsync(string username){ // THIS by me
            var existsUser = userRepository.GetUserByUsername(username) ?? throw new Exception("Someting went wrong."); // THIS

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return Task.FromResult<AuthResponse>(null);
            }
            if (!_refreshTokenToUsername.TryGetValue(refreshToken, out var username))
            {
                return Task.FromResult<AuthResponse>(null);
            }
            // rotate refresh token
            _refreshTokenToUsername.Remove(refreshToken);
            var newRefreshToken = GenerateRefreshToken();
            _refreshTokenToUsername[newRefreshToken] = username;

            var newAccessToken = GenerateJwtToken(Uuid);
            return Task.FromResult(new AuthResponse
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        private string GenerateJwtToken(Guid userUuid)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userUuid.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Use the secret key to create signing credentials.
            
            var credentials = _signingCredentials ?? new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecretKey)), SecurityAlgorithms.HmacSha256);

            // Create the token 
            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(60), // Token expires in 30 minutes.
                signingCredentials: credentials);

            // The handler writes the token into a string.
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        
        private static string GenerateRefreshToken()
        {
            var bytes = new byte[32];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}
