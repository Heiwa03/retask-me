
// System dependency
using System.Security.Cryptography;
using System.Text;

// Used namespaces
using BusinessLogicLayer.Services.Interfaces;
using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.testsBagrin;


namespace BusinessLogicLayer.Services{
    public class RegisterService : IRegisterService{
        IUserRep _userRep;

        public RegisterService(IUserRep _userRep){
            this._userRep = _userRep;
        }

        // 1. Main method for register
        public async Task RegisterUser(RegisterDTO dto){
            // Validate login / password / rep password
            ValidateRegisterData(dto);

            // HashPassword
            string hashedPassword = HashPassword(dto.Password);

            // Save in bd
            TestUser user = new TestUser{
                ID = 0,
                Login = dto.Login,
                HashedPassword = hashedPassword
            };

            await(_userRep.AddUser(user));
        }

        // 2. Validate rep passowrd
        private static void ValidateRegisterData(RegisterDTO dto){
            if(dto.Password != dto.RepeatPassword){
                throw new ArgumentException("Passwords do not match");
            }
        }

        // 3. Hash password
        private static string HashPassword(string password){
            using (SHA256 sha256 = SHA256.Create()){
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes); // !!!
                
                return Convert.ToBase64String(hash);
            }
        }
    }
}