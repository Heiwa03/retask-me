using BusinessLogicLayerCore.Services;
using DataAccessLayerCore.Repositories;
using Moq;
using Xunit;

namespace BusinessLogicLayerCoreTests.TestProfile;

    public class UserProfileFixture{

        private readonly Mock<UserRepository> _userRepository = new();
        private readonly ProfileService profileService;

        public UserProfileFixture(){
            profileService = new ProfileService(_userRepository.Object);
        }

    }