
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Services.Interfaces;
using DataAccessLayerCore;
using BusinessLogicLayerCore.Templates;
using HelperLayer.Security.Token;
using HelperLayer.Security;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly DatabaseContext _databaseContext;
    private readonly SigningCredentials _signingCredentials;
    private readonly string _frontendUrl;
    private readonly EmailHelper _emailHelper;

    public AuthController(
        IAuthService authService,
        DatabaseContext databaseContext,
        SigningCredentials signingCredentials,
        IConfiguration configuration,
        EmailHelper emailHelper)
    {
        _authService = authService;
        _databaseContext = databaseContext;
        _signingCredentials = signingCredentials;
        _frontendUrl = configuration["Frontend:BaseUrl"];
        _emailHelper = emailHelper;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var user = await _databaseContext.Users
            .FirstOrDefaultAsync(u => u.NormalizedUsername == loginDto.Email.ToUpperInvariant());

        if (user == null || !PasswordHelper.VerifyHashedPassword(loginDto.Password, user.Password))
            return Unauthorized(new { message = "Invalid email or password.", isVerified = false });

        if (!user.IsVerified)
        {
            // Optional: resend verification email
            string token = TokenHelper.GenerateJwtToken(
                user.NormalizedUsername,
                _signingCredentials,
                issuer: null,
                audience: null,
                expiresMinutes: 60
            );

            string verificationLink = $"{_frontendUrl}/api/v1/Email/verify-email?token={token}";

            string bodyContent = $@"
                <p>Hi,</p>
                <p>Please click the link below to verify your email:</p>
                <p><a href='{verificationLink}'>Verify Email</a></p>
                <p>If you did not register, ignore this email.</p>";

            string htmlContent = EmailTemplates.WelcomeTemplate(bodyContent);

            await _emailHelper.SendEmailAsync(
                new List<string> { user.Username }, // replace with actual email
                "Verify Your Email",
                htmlContent
            );

            return BadRequest(new { message = "Email not verified. A new verification link has been sent.", isVerified = false });
        }

        // If verified, generate JWT for session
        var authResponse = await _authService.LoginAsync(user.NormalizedUsername, loginDto.Password);

        return Ok(new { authResponse, isVerified = true });
    }
}
