using Microsoft.AspNetCore.Mvc;
using BusinessLogicLayerCore.Services.Interfaces;
using BusinessLogicLayerCore.DTOs;
using Microsoft.AspNetCore.Authorization;
using ReTaskMe.Models.Response;

namespace ReTaskMe.Controllers;
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController(IProfileService _profileService) : BaseController {
        
        [HttpPost("registerProfile")]
        public async Task RegisterUserProfile([FromBody] PostRegisterDTO dto){
            await _profileService.RegisterUserProfile(dto, TestUserGuid ?? Guid.NewGuid());
        }

        [HttpGet("getUserProfile")]
        public async Task<UserProfileModel> GetProfile(){
            var profile = await _profileService.GetProfile(TestUserGuid ?? Guid.NewGuid());

            var profileModel = new UserProfileModel{
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                Gender = profile.Gender
            };

            return profileModel;
        }

        [HttpPost("updateRegisterProfile")]
        public async Task UpdateProfile([FromBody] PostRegisterDTO dto){
            await _profileService.UpdateProfile(dto, TestUserGuid ?? Guid.NewGuid());
        }
    

    }

