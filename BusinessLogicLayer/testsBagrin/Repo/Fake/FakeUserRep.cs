

// System dependency
using System.Text.Json.Serialization.Metadata;

// BL
using BusinessLogicLayer.testsBagrin.Entity;

// HL
using BusinessLogicLayer.testsBagrin.Interfaces;

// DAL
using DataAccessLayer.Repositories.Interfaces;


namespace BusinessLogicLayer.testsBagrin.Repo.Fake{
    public class FakeUserRep{
        private readonly List<TestUser> _users = new();
        private readonly IUserRepository _userRepository;
        public FakeUserRep(IUserRepository _userRepository){
        }
    }
}
