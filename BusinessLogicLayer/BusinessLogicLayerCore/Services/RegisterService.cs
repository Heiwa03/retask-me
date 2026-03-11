using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BusinessLogicLayerCore.Services.Interfaces;
using BusinessLogicLayerCore.DTOs;
using DataAccessLayerCore.Entities;
using HelperLayer.Security;
using HelperLayer.Security.Token;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using DataAccessLayerCore.Repositories.Interfaces;
using BusinessLogicLayerCore.Exceptions;

namespace BusinessLogicLayerCore.Services
{
    public class RegisterService : IRegisterService
    {
        private readonly IUserRepository _userRepository;
        private readonly SigningCredentials _signingCredentials;
        private readonly string _frontendUrl;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;

        public RegisterService(
            IUserRepository userRepository,
            SigningCredentials signingCredentials,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _signingCredentials = signingCredentials;

            _frontendUrl = configuration["Frontend:BaseUrl"]
                           ?? throw new ArgumentNullException("Frontend:BaseUrl missing");

            _jwtIssuer = configuration["Authorization:Issuer"] ?? throw new ArgumentNullException("Authorization:Issuer missing");
            _jwtAudience = configuration["Authorization:Audience"] ?? throw new ArgumentNullException("Authorization:Audience missing");
        }

        public async Task RegisterUser(RegisterDTO dto)
        {
            // --- Input validation ---
            if (_userRepository.IsUsernameOccupied(dto.Mail))
                //throw new InvalidOperationException("Email already exists.");  !!! Old exception
                throw new UsernameExistException();

            if (!PasswordHelper.ValidateRegisterData(dto.Password, dto.RepeatPassword))
                //throw new InvalidOperationException("Passwords do not match.");  !!! Old exception
                throw new MatchPasswordException(); // TODO: MatchPasswordHandler

            if (!PasswordHelper.IsPasswordStrong(dto.Password))
                //throw new InvalidOperationException("Password is not motivated enough.");
                throw new ValidPasswordException();

            // --- Create user ---
            string hashedPassword = PasswordHelper.HashPassword(dto.Password);

            var user = new User

            {
                Uuid = Guid.NewGuid(),
                Username = dto.Mail,
                NormalizedUsername = dto.Mail.ToUpperInvariant(),
                Password = hashedPassword,
                IsVerified = false
            };

            _userRepository.Add(user);
            await _userRepository.SaveChangesAsync();

            // --- Create session ---
            var session = new UserSession
            {
                Uuid = user.Uuid,
                User = user,
                UserId = user.Id,
                RefreshToken = TokenHelper.GenerateRefreshToken(),
                JwtId = user.Uuid.ToString(),
                RefreshTokenExpiration = DateTime.UtcNow.AddDays(7),
                Redeemed = false
            };
            _userRepository.Add(session);
            await _userRepository.SaveChangesAsync();

            // --- Generate JWT verification token (1h expiry) ---
            string token = TokenHelper.GenerateJwtToken(
                user.Uuid,
                user.NormalizedUsername,
                _signingCredentials,
                issuer: _jwtIssuer,
                audience: _jwtAudience,
                expiresMinutes: 60
            );
        }
    }
}
