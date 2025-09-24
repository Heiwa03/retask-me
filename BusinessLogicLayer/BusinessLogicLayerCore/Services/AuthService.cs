using BusinessLogicLayerCore.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using HelperLayer.Security.Token;
namespace BusinessLogicLayer.Services
{
    public class AuthService(IUserRepository userRepository, ILoginChecker loginChecker,IConfiguration configuration, SigningCredentials signingCredentials) : IAuthService
    {
        private readonly ILoginChecker _loginChecker;

        // JWT Signing
        private readonly SigningCredentials _signingCredentials;
        private readonly string? _issuer;
        private readonly string? _audience;
        private static readonly Dictionary<string, string> _refreshTokenToEmail = new();


        public AuthService(ILoginChecker loginChecker, IConfiguration configuration, SigningCredentials signingCredentials)
        {
            _loginChecker = loginChecker;
            _signingCredentials = signingCredentials;
            _issuer = configuration["Authorization:Issuer"];
            _audience = configuration["Authorization:Audience"];
        }

        public Task<AuthResponse> LoginAsync(string email, string password)
        {
            if (_loginChecker.CheckCredentials(email, password))
            {
                var token = TokenHelper.GenerateJwtToken(email, _signingCredentials, _issuer, _audience, 60);
                var refreshToken = TokenHelper.GenerateRefreshToken();
                _refreshTokenToEmail[refreshToken] = email;
                return Task.FromResult(new AuthResponse { Token = token, RefreshToken = refreshToken });
            }

            return new AuthResponse();
        }

        public Task<AuthResponse> RefreshAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return Task.FromResult<AuthResponse>(null);
            }
            if (!_refreshTokenToEmail.TryGetValue(refreshToken, out var email))
            {
                return Task.FromResult<AuthResponse>(null);
            }
            // rotate refresh token
            _refreshTokenToEmail.Remove(refreshToken);
            var newRefreshToken = TokenHelper.GenerateRefreshToken();
            _refreshTokenToEmail[newRefreshToken] = email;

            var newAccessToken = TokenHelper.GenerateJwtToken(email, _signingCredentials, _issuer, _audience, 60);
            return Task.FromResult(new AuthResponse
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }
    }
}
