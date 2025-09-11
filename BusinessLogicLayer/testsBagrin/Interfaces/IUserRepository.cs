
using BusinessLogicLayer.testsBagrin.Entity;

namespace BusinessLogicLayer.testsBagrin.Interfaces{
    public interface IUserRepository{
        Task AddUser(TestUser user);
        List<TestUser> GetAllUsers();
        List<TestUser> GetAllMails();
    }
}