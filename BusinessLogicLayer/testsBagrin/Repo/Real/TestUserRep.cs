

using BusinessLogicLayer.testsBagrin.Interfaces;
using BusinessLogicLayer.testsBagrin.Entity;


namespace BusinessLogicLayer.testsBagrin.Repo.Real{
    public class TestUserRep { // IUserRepository
        private readonly TestConnector _context;

        public TestUserRep(TestConnector _context){
            this._context = _context;
        }

        public async Task AddUser(TestUser user){
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
    }
}