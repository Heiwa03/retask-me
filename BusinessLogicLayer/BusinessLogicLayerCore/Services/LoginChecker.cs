using BusinessLogicLayerCore.DTOs;
using BusinessLogicLayerCore.Services.Interfaces;
using DataAccessLayerCore.Repositories.Interfaces;
using HelperLayer.Security;

namespace BusinessLogicLayerCore.Services
{
    public class LoginChecker : ILoginChecker
    {
        private readonly IUserRepository _userRepository;

        public LoginChecker(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> CheckCredentials(LoginDto dto)
        {
            var user = await _userRepository.GetUserByUsername(dto.Email);
            if (user == null)
                return false;

            return PasswordHelper.VerifyHashedPassword(dto.Password, user.Password);
        }
    }
}
