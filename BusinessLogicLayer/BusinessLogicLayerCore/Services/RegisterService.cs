// System dependency
using Microsoft.EntityFrameworkCore;

// Used namespaces from BL
using BusinessLogicLayerCore.Services.Interfaces;
using BusinessLogicLayerCore.DTOs;

// Used namespaces from HL
using HelperLayer.Security;
using HelperLayer.Security.Token;

// Used namespaces from DAL
using DataAccessLayerCore.Repositories.Interfaces;
using DataAccessLayerCore.Entities;

namespace BusinessLogicLayerCore.Services
{
    /// <summary>
    /// Service responsible for user registration.
    /// </summary>
    public class RegisterService : IRegisterService
    {
        private readonly IUserRepository _userRepository;
        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterService"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="baseRepository">The base repository for database operations.</param>
        public RegisterService(IUserRepository _userRepository)
        {
            this._userRepository = _userRepository;
        }

        /// <summary>
        /// Main method for registering a user.
        /// Checks username uniqueness, password strength, password match,
        /// hashes the password, creates the user and session.
        /// </summary>
        /// <param name="dto">The registration DTO containing user data.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RegisterUser(RegisterDTO dto)
        {
            // Check if mail is unique
            CheckUniqueMail(dto.Mail);

            // Validate password repeat
            CheckRepeatPassword(dto.Password, dto.RepeatPassword);
            
            // Validate password strength
            CheckPasswordRequirements(dto.Password);

            // Hashing Password with BCrypt
            string hashedPassword = PasswordHelper.HashPassword(dto.Password);

            // Create user 
            User user = CreateUser(dto, hashedPassword);
            _userRepository.Add(user); // base
            await _userRepository.SaveChangesAsync(); // base

            // Create session
            UserSession userSession = CreateSession(user);
            _userRepository.Add(userSession); //base
            await SaveChanges();
        }

        /// <summary>
        /// Checks if the mail is unique.
        /// </summary>
        /// <param name="mail">The mail to check.</param>
        /// <exception cref="InvalidOperationException">Thrown if the mail is already taken.</exception>
        public void CheckUniqueMail(string mail) // for now
        {
            if (_userRepository.IsUserNameOccupied(mail))
            {
                throw new InvalidOperationException("Username already exists");
            }
        }

        /// <summary>
        /// Checks that the repeated password matches the original password.
        /// </summary>
        /// <param name="password">The original password.</param>
        /// <param name="repeatPassword">The repeated password.</param>
        /// <exception cref="InvalidOperationException">Thrown if passwords do not match.</exception>
        public void CheckRepeatPassword(string password, string repeatPassword)
        {
            if (!PasswordHelper.ValidateRegisterData(password, repeatPassword))
            {
                throw new InvalidOperationException("Password does not match");
            }
        }

        /// <summary>
        /// Checks the strength of the password.
        /// </summary>
        /// <param name="password">The password to check.</param>
        /// <exception cref="InvalidOperationException">Thrown if the password is not strong.</exception>
        public void CheckPasswordRequirements(string password)
        {
            if (!PasswordHelper.IsPasswordStrong(password))
            {
                throw new InvalidOperationException("Password is not strong");
            }
        }

        /// <summary>
        /// Creates a <see cref="User"/> object.
        /// </summary>
        /// <param name="dto">The registration DTO.</param>
        /// <param name="hashedPassword">The hashed password.</param>
        /// <returns>The created <see cref="User"/> object.</returns>
        public static User CreateUser(RegisterDTO dto, string hashedPassword)
        {
            User user = new User
            {
                Id = 0,
                Uuid = Guid.NewGuid(),
                Username = dto.Mail,
                NormalizedUsername = dto.Mail.ToUpperInvariant(),
                Password = hashedPassword
            };
            return user;
        }

        /// <summary>
        /// Creates a <see cref="UserSession"/> object for the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>The created <see cref="UserSession"/> object.</returns>
        public static UserSession CreateSession(User user)
        {
            string generatedRefreshToken = TokenHelper.GenerateRefreshToken();

            UserSession userSession = new UserSession
            {
                Id = 0,
                Uuid = user.Uuid,
                User = user,
                UserId = user.Id,
                RefreshToken = generatedRefreshToken,
                JwtId = user.Uuid.ToString(),
                RefreshTokenExpiration = DateTime.UtcNow.AddDays(7),
                Redeemed = false
            };

            return userSession;
        }

        /// <summary>
        /// Saves changes in the database asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="DbUpdateException">Thrown if an error occurs during saving.</exception>
        public async Task SaveChanges()
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
    }
}
