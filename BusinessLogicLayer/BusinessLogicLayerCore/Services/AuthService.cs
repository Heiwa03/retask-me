
using BusinessLogicLayerCore.DTOs;
using BusinessLogicLayerCore.Services.Interfaces;
using DataAccessLayerCore.Entities;
using DataAccessLayerCore.Repositories.Interfaces;
using HelperLayer.Security;
using HelperLayer.Security.Token;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BusinessLogicLayerCore.Exceptions;


namespace BusinessLogicLayerCore.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserSessionRepository _userSessionRepository;

        private readonly SigningCredentials _signingCredentials;
        private readonly string _issuer;
        private readonly string _audience;
        private const int AccessTokenMinutes = 60;
        private const int RefreshTokenDays = 7;

        public AuthService(
            IUserRepository userRepository,
            IUserSessionRepository userSessionRepository,
            IConfiguration configuration,
            SigningCredentials signingCredentials)
        {
            _userRepository = userRepository;
            _userSessionRepository = userSessionRepository;
            _signingCredentials = signingCredentials;
            _issuer = configuration["Jwt:Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer");
            _audience = configuration["Jwt:Audience"] ?? throw new ArgumentNullException("Jwt:Audience");
        }

        public async Task<AuthResponse> LoginAsync(LoginDto loginDto){
            if (string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password)){
                throw new InvalidCredentialsException(); 
            }

            var user = await _userRepository.GetUserByUsername(loginDto.Email);

            if(user == null){
                throw new InvalidCredentialsException();
            }

            if(!PasswordHelper.VerifyHashedPassword(loginDto.Password, user.Password)){
                throw new InvalidCredentialsException();
            }

            return await CreateSessionAsync(user);
        }

        private async Task<AuthResponse> CreateSessionAsync(User user){
            await _userSessionRepository.RemoveSessionByUserIdAsync(user.Id);

            var accessToken = TokenHelper.GenerateJwtToken(user.Uuid, user.NormalizedUsername, _signingCredentials, _issuer, _audience, AccessTokenMinutes);
            var refreshToken = TokenHelper.GenerateRefreshToken();

            var session = new UserSession
            {
                UserId = user.Id,
                Uuid = user.Uuid,
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


        public async Task<AuthResponse?> RefreshAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return null;

            var existingSession = await _userSessionRepository.GetSessionByRefreshTokenAsync(refreshToken);
            if (existingSession == null || existingSession.Redeemed || existingSession.RefreshTokenExpiration <= DateTime.UtcNow)
                return null;

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


            var accessToken = TokenHelper.GenerateJwtToken(user.Uuid, user.NormalizedUsername, _signingCredentials, _issuer, _audience, AccessTokenMinutes);

            return new AuthResponse
            {
                Token = accessToken,
                RefreshToken = newRefreshToken
            };
        }
    }
}
