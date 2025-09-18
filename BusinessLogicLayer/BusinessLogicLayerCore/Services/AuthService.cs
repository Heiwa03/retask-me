using BusinessLogicLayer.Services;
using BusinessLogicLayer.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using HelperLayer.Security.Token;
namespace BusinessLogicLayer.Services
{
    public class AuthService : IAuthService
    {
        private readonly ILoginChecker _loginChecker;

        // JWT Signing
        private readonly SigningCredentials _signingCredentials;
        private readonly string? _issuer;
        private readonly string? _audience;
        private static readonly Dictionary<string, string> _refreshTokenToUsername = new();

        public AuthService(ILoginChecker loginChecker, IConfiguration configuration, SigningCredentials signingCredentials)
        {
            _loginChecker = loginChecker;
            _signingCredentials = signingCredentials;
            _issuer = configuration["Authorization:Issuer"];
            _audience = configuration["Authorization:Audience"];
        }

        public Task<AuthResponse> LoginAsync(string username, string password)
        {
            if (_loginChecker.CheckCredentials(username, password))
            {
                var token = TokenHelper.GenerateJwtToken(username, _signingCredentials, _issuer, _audience, 60);
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

            var newAccessToken = TokenHelper.GenerateJwtToken(username, _signingCredentials, _issuer, _audience, 60);
            return Task.FromResult(new AuthResponse
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }
    }
}
