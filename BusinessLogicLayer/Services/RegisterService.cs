
// System dependency
using System.Security.Cryptography;
using System.Text;

// Used namespaces
using BusinessLogicLayer.Services.Interfaces;
using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.testsBagrin.Entity;
using BusinessLogicLayer.testsBagrin.Interfaces;

using HelperLayer.Security;


namespace BusinessLogicLayer.Services{
    public class RegisterService : IRegisterService{
        IUserRep _userRep;

        public RegisterService(IUserRep _userRep){
            this._userRep = _userRep;
        }

        // 1. Main method for register
        public async Task RegisterUser(RegisterDTO dto){
            // Validate login / password / rep password
            PasswordHelper.ValidateRegisterData(dto.Password, dto.RepeatPassword);

            // HashPassword
            string hashedPassword = PasswordHelper.HashPassword(dto.Password);

            // Save in bd
            TestUser user = new TestUser{
                ID = 0,
                Login = dto.Login,
                HashedPassword = hashedPassword
            };

            await(_userRep.AddUser(user));
        }
    }
}