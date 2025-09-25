using BusinessLogicLayerCore.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using HelperLayer.Security.Token;
using System.Collections.Concurrent;
using DataAccessLayerCore.Repositories.Interfaces;
using DataAccessLayerCore.Entities;
namespace BusinessLogicLayerCore.Services
{
    public class AuthService : IAuthService
    {
        private readonly ILoginChecker _loginChecker;
        private readonly IUserRepository _userRepository;
        private readonly IUserSessionRepository _userSessionRepository;

        // JWT Signing
        private readonly SigningCredentials _signingCredentials;
        private readonly string? _issuer;
        private readonly string? _audience;

        public AuthService(
            ILoginChecker loginChecker,
            IUserRepository userRepository,
            IUserSessionRepository userSessionRepository,
            IConfiguration configuration,
            SigningCredentials signingCredentials)
        {
            _loginChecker = loginChecker;
            _userRepository = userRepository;
            _userSessionRepository = userSessionRepository;
            _signingCredentials = signingCredentials;
            _issuer = configuration["Authorization:Issuer"];
            _audience = configuration["Authorization:Audience"];
        }

        public Task<AuthResponse?> LoginAsync(string email, string password)
        {
            if (!_loginChecker.CheckCredentials(email, password))
            {
                return Task.FromResult<AuthResponse?>(null);
            }

            return LoginWithPersistentSessionAsync(email);
        }

        public Task<AuthResponse?> RefreshAsync(string refreshToken)
        {
            return RefreshWithPersistentSessionAsync(refreshToken);
        }

        private async Task<AuthResponse?> LoginWithPersistentSessionAsync(string email)
        {
            var user = await _userRepository.GetUserByUsername(email);
            if (user == null)
            {
                return null;
            }

            // Remove existing active sessions for this user
            await _userSessionRepository.RemoveSessionByUserIdAsync(user.Id);

            var accessToken = TokenHelper.GenerateJwtToken(email, _signingCredentials, _issuer, _audience, 60);
            var refreshToken = TokenHelper.GenerateRefreshToken();

            var session = new UserSession
            {
                Id = 0,
                Uuid = user.Uuid,
                User = user,
                UserId = user.Id,
                RefreshToken = refreshToken,
                JwtId = user.Uuid.ToString(),
                RefreshTokenExpiration = DateTime.UtcNow.AddDays(7),
                Redeemed = false
            };

            _userSessionRepository.Add(session);
            await _userSessionRepository.SaveChangesAsync();

            return new AuthResponse
            {
                Token = accessToken,
                RefreshToken = refreshToken
            };
        }

        private async Task<AuthResponse?> RefreshWithPersistentSessionAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return null;
            }

            var existingSession = await _userSessionRepository.GetSessionByRefreshTokenAsync(refreshToken);
            if (existingSession == null)
            {
                return null;
            }

            // Validate not redeemed and not expired
            if (existingSession.Redeemed || existingSession.RefreshTokenExpiration <= DateTime.UtcNow)
            {
                return null;
            }

            // Redeem old token and rotate
            var newJwtId = Guid.NewGuid().ToString();
            var redeemed = await _userSessionRepository.RedeemRefreshTokenAsync(refreshToken, newJwtId);
            if (!redeemed)
            {
                return null;
            }
            await _userSessionRepository.SaveChangesAsync();

            // Issue new refresh token as a new session
            var newRefreshToken = TokenHelper.GenerateRefreshToken();
            var user = existingSession.User;

            var newSession = new UserSession
            {
                Id = 0,
                Uuid = user.Uuid,
                User = user,
                UserId = user.Id,
                RefreshToken = newRefreshToken,
                JwtId = newJwtId,
                RefreshTokenExpiration = DateTime.UtcNow.AddDays(7),
                Redeemed = false
            };
            _userSessionRepository.Add(newSession);
            await _userSessionRepository.SaveChangesAsync();

            var newAccessToken = TokenHelper.GenerateJwtToken(user.NormalizedUsername, _signingCredentials, _issuer, _audience, 60);
            return new AuthResponse
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }
    }
}
