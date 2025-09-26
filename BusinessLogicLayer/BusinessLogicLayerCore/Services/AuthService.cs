using BusinessLogicLayerCore.Services.Interfaces;
using DataAccessLayerCore.Entities;
using DataAccessLayerCore.Repositories.Interfaces;
using HelperLayer.Security.Token;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BusinessLogicLayerCore.Services
{
    public class AuthService : IAuthService
    {
        private readonly ILoginChecker _loginChecker;
        private readonly IUserRepository _userRepository;
        private readonly IUserSessionRepository _userSessionRepository;

        private readonly SigningCredentials _signingCredentials;
        private readonly string _issuer;
        private readonly string _audience;

        private const int AccessTokenMinutes = 60;
        private const int RefreshTokenDays = 7;

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
            _issuer = configuration["Authorization:Issuer"] ?? throw new ArgumentNullException("Authorization:Issuer");
            _audience = configuration["Authorization:Audience"] ?? throw new ArgumentNullException("Authorization:Audience");
        }

        public async Task<AuthResponse?> LoginAsync(string email, string password)
        {
            if (!await _loginChecker.CheckCredentials(email, password))
                return null;

            return await CreateSessionAsync(email);
        }

        public async Task<AuthResponse?> RefreshAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return null;

            var existingSession = await _userSessionRepository.GetSessionByRefreshTokenAsync(refreshToken);
            if (existingSession == null || existingSession.Redeemed || existingSession.RefreshTokenExpiration <= DateTime.UtcNow)
                return null;

            // Redeem and rotate session atomically
            var newJwtId = Guid.NewGuid().ToString();
            bool redeemed = await _userSessionRepository.RedeemRefreshTokenAsync(refreshToken, newJwtId);
            if (!redeemed)
                return null;

            var user = existingSession.User;

            var newRefreshToken = TokenHelper.GenerateRefreshToken();
            var newSession = new UserSession
            {
                UserId = user.Id,
                Uuid = user.Uuid,
                User = user,
                RefreshToken = newRefreshToken,
                JwtId = newJwtId,
                RefreshTokenExpiration = DateTime.UtcNow.AddDays(RefreshTokenDays),
                Redeemed = false
            };

            _userSessionRepository.Add(newSession);
            await _userSessionRepository.SaveChangesAsync();

            var accessToken = TokenHelper.GenerateJwtToken(user.NormalizedUsername, _signingCredentials, _issuer, _audience, AccessTokenMinutes);

            return new AuthResponse
            {
                Token = accessToken,
                RefreshToken = newRefreshToken
            };
        }

        private async Task<AuthResponse> CreateSessionAsync(string email)
        {
            var user = await _userRepository.GetUserByUsername(email);
            if (user == null)
                throw new InvalidOperationException("User not found after credential check.");

            // Remove any existing sessions for this user
            await _userSessionRepository.RemoveSessionByUserIdAsync(user.Id);

            var accessToken = TokenHelper.GenerateJwtToken(user.NormalizedUsername, _signingCredentials, _issuer, _audience, AccessTokenMinutes);
            var refreshToken = TokenHelper.GenerateRefreshToken();

            var session = new UserSession
            {
                UserId = user.Id,
                Uuid = user.Uuid,
                //User = user,
                RefreshToken = refreshToken,
                JwtId = user.Uuid.ToString(),
                RefreshTokenExpiration = DateTime.UtcNow.AddDays(RefreshTokenDays),
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
    }
}
