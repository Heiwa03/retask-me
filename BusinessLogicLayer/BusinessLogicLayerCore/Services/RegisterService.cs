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

    public class RegisterService : IRegisterService
    {
        private readonly IUserRepository _userRepository;
            
        public RegisterService(IUserRepository _userRepository)
        {
            this._userRepository = _userRepository;
        }

        public async Task RegisterUser(RegisterDTO dto){
        // Проверка уникальности
        if (_userRepository.IsUserNameOccupied(dto.Mail))
            throw new InvalidOperationException("Username already exists");

        // Проверка совпадения паролей
        if (!PasswordHelper.ValidateRegisterData(dto.Password, dto.RepeatPassword))
            throw new InvalidOperationException("Password does not match");

        // Проверка силы пароля
        if (!PasswordHelper.IsPasswordStrong(dto.Password))
            throw new InvalidOperationException("Password is not strong");

        // Хэшируем пароль
        string hashedPassword = PasswordHelper.HashPassword(dto.Password);

        // Создаём пользователя
        User user = new User
        {
            Id = 0,
            Uuid = Guid.NewGuid(),
            Username = dto.Mail,
            NormalizedUsername = dto.Mail.ToUpperInvariant(),
            Password = hashedPassword
        };
        _userRepository.Add(user);

        // Создаём сессию
        UserSession session = new UserSession
        {
            Id = 0,
            Uuid = user.Uuid,
            User = user,
            UserId = user.Id,
            RefreshToken = TokenHelper.GenerateRefreshToken(),
            JwtId = user.Uuid.ToString(),
            RefreshTokenExpiration = DateTime.UtcNow.AddDays(7),
            Redeemed = false
        };
        _userRepository.Add(session);

        // Сохраняем всё одним вызовом
        await _userRepository.SaveChangesAsync();
        }
    }
}