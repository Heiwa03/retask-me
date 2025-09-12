

// System dependency
using System.Text.Json.Serialization.Metadata;

// BL
using BusinessLogicLayer.testsBagrin.Entity;

// HL
using BusinessLogicLayer.testsBagrin.Interfaces;


namespace BusinessLogicLayer.testsBagrin.Repo.Fake{
    public class FakeUserRep : IUserRepository{
        private readonly List<TestUser> _users = new();
        
        public Task AddUser(TestUser user){
            _users.Add(user);
            return Task.CompletedTask;
        }

        // Get all users
        public List<TestUser> GetAllUsers() => _users;
        
        // Get all mails
        public List<TestUser> GetAllMails() => _users;

        // Sniff date from "db"
        public void Print(){
            foreach (var user in _users){
                Console.WriteLine($"Username: {user.Username}, Password {user.HashedPassword}, Mail: {user.Mail}, Token: {user.RefreshToken}");
            }
        }
    }
}
