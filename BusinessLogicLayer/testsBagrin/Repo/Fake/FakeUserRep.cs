using BusinessLogicLayer.testsBagrin.Entity;

using BusinessLogicLayer.testsBagrin.Interfaces;

namespace BusinessLogicLayer.testsBagrin.Repo.Fake{
    public class FakeUserRep : IUserRep{
        private readonly List<TestUser> _users = new();

        public Task AddUser(TestUser user){
            _users.Add(user);
            return Task.CompletedTask;
        }

        public List<TestUser> GetAllUsers() => _users;
    }
}
