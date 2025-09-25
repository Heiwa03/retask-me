using BusinessLogicLayerCore.DTOs;
using BusinessLogicLayerCore.Services.Interfaces;
using DataAccessLayerCore.Entities;
using DataAccessLayerCore.Repositories.Interfaces;

namespace BusinessLogicLayerCore.Services;

    public class ProfileService(IUserRepository _userRepository) : IProfileService {
        public async Task<PostRegisterDTO> GetProfile(Guid uuid){
            var profile = await _userRepository.GetByUuidAsync<User>(uuid);

            if (profile == null) {
                return new PostRegisterDTO();
            }
            
            return new PostRegisterDTO {
                Username = profile.Username,
                FirstName = profile.FirstName ?? "Nofirstname",
                LastName = profile.LastName ?? "Nolastname",
                Gender = profile.Gender
            };
        }   

        public async Task UpdateProfile(PostRegisterDTO dto, Guid userUid){
            User? profile = await _userRepository.GetByUuidAsync<User>(userUid) ?? throw new KeyNotFoundException($"User not found");
            
            profile.Username = dto.Username;
            profile.FirstName = dto.FirstName;
            profile.LastName = dto.LastName;
            profile.Gender = dto.Gender;
        }  
    }