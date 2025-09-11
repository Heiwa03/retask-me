using BusinessLogicLayer.testsBagrin.Entity;

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
    }
}
