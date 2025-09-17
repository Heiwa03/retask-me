using BusinessLogicLayer.Services;
using BusinessLogicLayer.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using HelperLayer.Security.Token;
namespace BusinessLogicLayer.Services
{
    public class AuthService : IAuthService
    {
        private readonly ILoginChecker _loginChecker;

        // JWT Signing key
    
        private readonly string _jwtSecretKey;
        private readonly SigningCredentials _signingCredentials;
        private readonly string? _issuer;
        private readonly string? _audience;
        private static readonly Dictionary<string, string> _refreshTokenToUsername = new();

        public AuthService(ILoginChecker loginChecker, IConfiguration configuration, SigningCredentials signingCredentials)
        {
            _loginChecker = loginChecker;
            _jwtSecretKey = configuration["JwtSecret"];
            _signingCredentials = signingCredentials;
            _issuer = configuration["Authorization:Issuer"];
            _audience = configuration["Authorization:Audience"];

            if (string.IsNullOrEmpty(_jwtSecretKey))
            {
                throw new ApplicationException("JWT secret key is not configured");
            }
        }

        public Task<AuthResponse> LoginAsync(string username, string password)
        {
            if (_loginChecker.CheckCredentials(username, password))
            {
                var token = TokenHelper.GenerateJwtToken(username, _signingCredentials, _issuer, _audience, 30);
                var refreshToken = TokenHelper.GenerateRefreshToken();
                _refreshTokenToUsername[refreshToken] = username;
                return Task.FromResult(new AuthResponse { Token = token, RefreshToken = refreshToken });
            }

            return Task.FromResult<AuthResponse>(null);
        }

        public Task<AuthResponse> RefreshAsync(string refreshToken)
        {
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
            var newRefreshToken = TokenHelper.GenerateRefreshToken();
            _refreshTokenToUsername[newRefreshToken] = username;

            var newAccessToken = TokenHelper.GenerateJwtToken(username, _signingCredentials, _issuer, _audience, 30);
            return Task.FromResult(new AuthResponse
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }
        
    }
}
