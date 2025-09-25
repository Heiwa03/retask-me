using BusinessLogicLayerCore.Services.Interfaces;
using DataAccessLayerCore.Repositories.Interfaces;
using HelperLayer.Security;

namespace BusinessLogicLayerCore.Services
{
    public class LoginChecker(IUserRepository userRepository) : ILoginChecker
    {
        private readonly IUserRepository _userRepository = userRepository;

        public bool CheckCredentials(string username, string password)
        {
            // Synchronous facade over async repo for simplicity in interface contract
            var user = _userRepository.GetUserByUsername(username).GetAwaiter().GetResult();
            if (user == null)
            {
                return false;
            }

            return PasswordHelper.VerifyHashedPassword(password, user.Password);
        }
    }
}


