
using BusinessLogicLayer.testsBagrin.Entity;

namespace BusinessLogicLayer.testsBagrin.Interfaces{
    public interface IUserRep{
        Task AddUser(TestUser user);
    }
}