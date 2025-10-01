using System.Text;
using BusinessLogicLayerCore.DTOs;
using BusinessLogicLayerCore.Services.Interfaces;
using DataAccessLayerCore.Entities;
using DataAccessLayerCore.Repositories.Interfaces;


namespace BusinessLogicLayerCore.Services;

    public class ProfileService(IUserRepository _userRepository) : IProfileService {

        public async Task RegisterUserProfile(PostRegisterDTO dto, Guid userUuid){
            var user = await _userRepository.GetByUuidAsync<User>(userUuid);

            if (user == null)
                throw new KeyNotFoundException($"User {userUuid} not found");

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Gender = dto.Gender;

            

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
        }


        public async Task<PostRegisterDTO> GetProfile(Guid uuid){
            var profile = await _userRepository.GetByUuidAsync<User>(uuid);

            if (profile == null) {
                return new PostRegisterDTO();
            }
            
            return new PostRegisterDTO {
                FirstName = profile.FirstName ?? "Nofirstname",
                LastName = profile.LastName ?? "Nolastname",
                Gender = profile.Gender
            };
        }   

        public async Task UpdateProfile(PostRegisterDTO dto, Guid userUid){
            User? profile = await _userRepository.GetByUuidAsync<User>(userUid) ?? throw new KeyNotFoundException($"User not found");
            
            profile.FirstName = dto.FirstName;
            profile.LastName = dto.LastName;
            profile.Gender = dto.Gender;
        }  
    }