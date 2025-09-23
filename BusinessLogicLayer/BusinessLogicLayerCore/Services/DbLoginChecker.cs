using BusinessLogicLayer.Services.Interfaces;
using DataAccessLayerCore.Repositories.Interfaces;
using HelperLayer.Security;

namespace BusinessLogicLayer.Services
{
    public class DbLoginChecker : ILoginChecker
    {
        private readonly IUserRepository _userRepository;

        public DbLoginChecker(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public bool CheckCredentials(string email, string password)
        {
            var user = _userRepository.GetUserByEmail(email).GetAwaiter().GetResult();
            if (user == null)
            {
                return false;
            }
            return PasswordHelper.VerifyHashedPassword(password, user.Password);
        }
    }
}


