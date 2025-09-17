using BusinessLogicLayer.Services.Interfaces;
using DataAccessLayer.Repositories.Interfaces;
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

        public bool CheckCredentials(string username, string password)
        {
            var user = _userRepository.GetUserByUsername(username).GetAwaiter().GetResult();
            if (user == null)
            {
                return false;
            }
            return PasswordHelper.VerifyHashedPassword(password, user.Password);
        }
    }
}


