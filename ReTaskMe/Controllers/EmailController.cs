using Microsoft.AspNetCore.Mvc;
using HelperLayer.Security.Token;
using DataAccessLayerCore.Repositories.Interfaces;

[ApiController]
[Route("api/v1/[controller]")]
public class EmailController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public EmailController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    /// <summary>
    /// Verifies a user's email by JWT token from the link
    /// </summary>
    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return BadRequest("Token is required.");

        //  Validate JWT
        string normalizedEmail;
        try
        {
            normalizedEmail = TokenHelper.ValidateJwtToken(token);
        }
        catch
        {
            return BadRequest("Invalid or expired token.");
        }

        //  Find user via repository
        var user = await _userRepository.GetUserByUsername(normalizedEmail);

        if (user == null)
            return NotFound("User not found.");

        //  Update verification state
        if (user.IsVerified)
            return Ok("Email already verified.");

        user.IsVerified = true;
        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        return Ok("Email successfully verified!");
    }
}
