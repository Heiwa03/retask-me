using BusinessLogicLayerCore.Services.Interfaces;

// PLACEHOLDER TEMPORARY LOGIN CHECKER
// MUST BE REMOVED ONCE DB IS READY
// WARNING NOT FOR SENSIBLE PEOPLE. O_O !!!HARD CODED VALUES!!! O_O

namespace BusinessLogicLayerCore.Services
{
    public class TemporaryLoginChecker : ILoginChecker
    {
        private const string _validUsername = "testuser";
        private const string _validPassword = "password123";

        public bool CheckCredentials(string username, string password)
        {
            // The logic to check credentials is now here.
            return username == _validUsername && password == _validPassword;
        }
    }
}
