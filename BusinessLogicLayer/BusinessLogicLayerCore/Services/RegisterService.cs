using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BusinessLogicLayerCore.Services.Interfaces;
using BusinessLogicLayerCore.DTOs;
using DataAccessLayerCore.Entities;
using HelperLayer.Security.Token;
using HelperLayer.Security;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

using DataAccessLayerCore.Repositories.Interfaces;

namespace BusinessLogicLayerCore.Services
{

    /// <summary>
    /// Service responsible for user registration and email verification.
    /// </summary>

    public class RegisterService : IRegisterService{
        private readonly IUserRepository _userRepository;

        private readonly IBaseRepository _baseRepository;
        private readonly IEmailService _emailService;
        private readonly SigningCredentials _signingCredentials;
        private readonly string _frontendUrl;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;

        public RegisterService(
            IUserRepository userRepository,
            IEmailService emailService,

            SigningCredentials signingCredentials,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _emailService = emailService;

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
                throw new InvalidOperationException("Email already exists.");

            if (!PasswordHelper.ValidateRegisterData(dto.Password, dto.RepeatPassword))
                throw new InvalidOperationException("Passwords do not match.");

            if (!PasswordHelper.IsPasswordStrong(dto.Password))
                throw new InvalidOperationException("Password is not strong enough.");

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
                user.NormalizedUsername,
                _signingCredentials,
                issuer: _jwtIssuer,
                audience: _jwtAudience,
                expiresMinutes: 60
            );

            string verificationLink = $"{_frontendUrl}/verify-email?token={token}";

            // --- Send verification email ---
            bool emailSent = await _emailService.SendVerificationEmailAsync(dto.Mail, verificationLink);

            if (!emailSent)

            {
                Console.WriteLine($"[RegisterService] Failed to send verification email to {dto.Mail}");
            }
        }
    }
}
