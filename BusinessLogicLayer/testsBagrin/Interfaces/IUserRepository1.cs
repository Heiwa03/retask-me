
using BusinessLogicLayer.testsBagrin.Entity;

namespace BusinessLogicLayer.testsBagrin.Interfaces{
    public interface IUserRepository1{
        Task AddUser(TestUser user);
        List<TestUser> GetAllUsers();
        List<TestUser> GetAllMails();
    }
}