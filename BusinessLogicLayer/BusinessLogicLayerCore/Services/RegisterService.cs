using Microsoft.EntityFrameworkCore;

// BL namespaces
using BusinessLogicLayerCore.Services.Interfaces;
using BusinessLogicLayerCore.DTOs;

// HL namespaces
using HelperLayer.Security;
using HelperLayer.Security.Token;

// DAL namespaces
using DataAccessLayerCore.Repositories.Interfaces;
using DataAccessLayerCore.Entities;
using BusinessLogicLayerCore.Templates;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

namespace BusinessLogicLayerCore.Services
{
    /// <summary>
    /// Service responsible for user registration and email verification.
    /// </summary>
    public class RegisterService : IRegisterService
    {
        private readonly IUserRepository _userRepository;
        private readonly IBaseRepository _baseRepository;
        private readonly IEmailService _emailService;
        private readonly SigningCredentials _signingCredentials;
        private readonly string _frontendUrl;

        public RegisterService(
            IUserRepository userRepository,
            IBaseRepository baseRepository,
            IEmailService emailService,
            SigningCredentials signingCredentials,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _baseRepository = baseRepository;
            _emailService = emailService;
            _signingCredentials = signingCredentials;
            _frontendUrl = configuration["Frontend:BaseUrl"] ?? throw new ArgumentNullException("Frontend:BaseUrl missing");
        }

        /// <summary>
        /// Registers a new user and sends verification email.
        /// </summary>
        public async Task RegisterUser(RegisterDTO dto)
        {
            // Validate input
            CheckUniqueMail(dto.Mail);
            CheckRepeatPassword(dto.Password, dto.RepeatPassword);
            CheckPasswordRequirements(dto.Password);

            // Hash password
            string hashedPassword = PasswordHelper.HashPassword(dto.Password);

            // Create user
            User user = CreateUser(dto, hashedPassword);
            _userRepository.Add(user);
            await _userRepository.SaveChangesAsync();

            // Create session
            UserSession userSession = CreateSession(user);
            _userRepository.Add(userSession);
            await SaveChanges();

            // Generate JWT verification token (1h expiry)
            string token = TokenHelper.GenerateJwtToken(
                user.NormalizedUsername,
                _signingCredentials,
                issuer: null,
                audience: null,
                expiresMinutes: 60
            );

            string verificationLink = $"{_frontendUrl}/verify-email?token={token}";

            // Delegate email creation & sending to EmailService
            await _emailService.SendVerificationEmailAsync(dto.Mail, verificationLink);
        }

        internal void CheckUniqueMail(string mail)
        {
            if (_userRepository.IsUsernameOccupied(mail))
                throw new InvalidOperationException("Email already exists");
        }

        internal void CheckRepeatPassword(string password, string repeatPassword)
        {
            if (!PasswordHelper.ValidateRegisterData(password, repeatPassword))
                throw new InvalidOperationException("Passwords do not match");
        }

        internal void CheckPasswordRequirements(string password)
        {
            if (!PasswordHelper.IsPasswordStrong(password))
                throw new InvalidOperationException("Password is not strong enough");
        }

        internal User CreateUser(RegisterDTO dto, string hashedPassword)
        {
            return new User
            {
                Id = 0,
                Uuid = Guid.NewGuid(),
                Username = dto.Mail,
                NormalizedUsername = dto.Mail.ToUpperInvariant(),
                Password = hashedPassword,
                IsVerified = false
            };
        }

        internal UserSession CreateSession(User user)
        {
            string refreshToken = TokenHelper.GenerateRefreshToken();

            return new UserSession
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
        }

        internal async Task SaveChanges()
        {
            try
            {
                await _userRepository.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.InnerException?.Message);
                throw;
            }
        }
    }
}
