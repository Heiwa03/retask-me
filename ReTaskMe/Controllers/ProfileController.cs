using Microsoft.AspNetCore.Mvc;
using BusinessLogicLayerCore.Services.Interfaces;
using BusinessLogicLayerCore.DTOs;
using Microsoft.AspNetCore.Authorization;
using ReTaskMe.Models.Response;

namespace ReTaskMe.Controllers;
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController(IUserService _userService, IProfileService _profileService) : BaseController {
        
        [HttpPost("registerProfile")]
        public async Task RegisterUserProfile([FromBody] PostRegisterDTO dto){
            await _profileService.RegisterUserProfile(dto, UserGuid ?? Guid.NewGuid());
        }

        [HttpGet("getUserProfile")]
        public async Task<UserProfileModel> GetProfile(){
            var profile = await _userService.GetUserProfile(UserGuid ?? Guid.NewGuid());

            var profileModel = new UserProfileModel{
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                Gender = profile.Gender
            };

            return profileModel;
        }

        [HttpPost("updateRegisterProfile")]
        public async Task UpdateProfile([FromBody] PostRegisterDTO dto){
            await _userService.UpdateUserProfile(dto, UserGuid ?? Guid.NewGuid());
        }
    

    }

