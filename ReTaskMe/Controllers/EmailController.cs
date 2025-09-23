using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HelperLayer.Security.Token;
using DataAccessLayerCore;
using DataAccessLayerCore.Entities;

[ApiController]
[Route("api/v1/[controller]")]
public class EmailController : ControllerBase
{
    private readonly DatabaseContext _dbContext;

    public EmailController(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Verifies a user's email by JWT token from the link
    /// </summary>
    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return BadRequest("Token is required.");

        // 1️⃣ Validate JWT
        string normalizedEmail;
        try
        {
            normalizedEmail = TokenHelper.ValidateJwtToken(token);
        }
        catch
        {
            return BadRequest("Invalid or expired token.");
        }

        // 2️⃣ Find user
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.NormalizedUsername == normalizedEmail.ToUpperInvariant());

        if (user == null)
            return NotFound("User not found.");

        // 3️⃣ Update verification state
        if (user.IsVerified)
            return Ok("Email already verified.");

        user.IsVerified = true;
        await _dbContext.SaveChangesAsync();

        return Ok("Email successfully verified!");
    }
}
