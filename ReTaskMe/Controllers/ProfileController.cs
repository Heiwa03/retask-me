using Microsoft.AspNetCore.Mvc;
using BusinessLogicLayerCore.Services.Interfaces;
using BusinessLogicLayerCore.DTOs;
using Microsoft.AspNetCore.Authorization;
using ReTaskMe.Models.Response;

namespace ReTaskMe.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize]    
public class ProfileController(IProfileService _profileService) : BaseController {

    [HttpPost("registerProfile")]
    public async Task<IActionResult> RegisterUserProfile([FromBody] PostRegisterDTO dto)
    {
        if (UserGuid is not Guid userGuid)
            return Unauthorized();

        await _profileService.RegisterUserProfile(dto, userGuid);
        return Ok(new { message = "Profile registered successfully" });
    }

    [HttpGet("getUserProfile")]
    public async Task<IActionResult> GetProfile()
    {
        if (UserGuid is not Guid userGuid)
            return Unauthorized(new { message = "User not authenticated" });

        var profile = await _profileService.GetProfile(userGuid);
        if (profile == null)
            return NotFound(new { message = "Profile not found" });

        var profileModel = new UserProfileModel
        {
            FirstName = profile.FirstName,
            LastName = profile.LastName,
            Gender = profile.Gender
        };

        return Ok(profileModel);
    }


    [HttpPost("updateRegisterProfile")]
    public async Task<IActionResult> UpdateProfile([FromBody] PostRegisterDTO dto)
    {
        if (UserGuid is not Guid userGuid)
            return Unauthorized(new { message = "User not authenticated" });

        try
        {
            await _profileService.UpdateProfile(dto, userGuid);
            return Ok(new { message = "Profile updated successfully" });
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(new { message = e.Message });
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message });
        }
    }

}

