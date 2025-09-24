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
        private readonly EmailHelper _emailHelper;
        private readonly SigningCredentials _signingCredentials;
        private readonly string _frontendUrl;

        public RegisterService(
            IUserRepository userRepository,
            IBaseRepository baseRepository,
            EmailHelper emailHelper,
            SigningCredentials signingCredentials,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _baseRepository = baseRepository;
            _emailHelper = emailHelper;
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

            //  Hash password
            string hashedPassword = PasswordHelper.HashPassword(dto.Password);

            // Create user
            User user = CreateUser(dto, hashedPassword);
            _userRepository.Add(user); // base
            await _userRepository.SaveChangesAsync(); // base

            // Create session
            UserSession userSession = CreateSession(user);
            _userRepository.Add(userSession); //base
            await SaveChanges();

            // Generate JWT verification token (1h expiry)
            string token = TokenHelper.GenerateJwtToken(
                user.NormalizedUsername, // or Email if added
                _signingCredentials,
                issuer: null,
                audience: null,
                expiresMinutes: 60
            );

            // Build verification link
            string verificationLink = $"{_frontendUrl}/verify-email?token={token}";

            // Build email content (Unicode-safe)
            string bodyContent = "<p>Hi,</p>" +
                                 "<p>Please click the link below to verify your email:</p>" +
                                 $"<p><a href='{verificationLink}'>Verify Email</a></p>" +
                                 "<p>If you did not register, ignore this email.</p>";

            string htmlContent = EmailTemplates.WelcomeTemplate(bodyContent);

            // Send verification email
            await _emailHelper.SendEmailAsync(
                new List<string> { dto.Mail },
                "Verify Your Email",
                htmlContent
            );
        }

        #region Internal helpers

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
                IsVerified = false // Ensure email not verified initially
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
                await _userRepository.SaveChangesAsync(); //base
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.InnerException?.Message);
                throw;
            }
        }

        #endregion

    }
}
