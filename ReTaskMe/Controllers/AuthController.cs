
﻿using Microsoft.AspNetCore.Mvc;
using BusinessLogicLayerCore.Services;
using BusinessLogicLayerCore.Services.Interfaces;
using BusinessLogicLayerCore.DTOs;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
    private readonly IEmailService _emailService;
    private readonly string _frontendUrl;

    public AuthController(
        IAuthService authService,
        DatabaseContext databaseContext,
        SigningCredentials signingCredentials,
        IConfiguration configuration,
        IEmailService emailService)
    {
        _authService = authService;
        _databaseContext = databaseContext;
        _signingCredentials = signingCredentials;
        _emailService = emailService;
        _frontendUrl = configuration["Frontend:BaseUrl"] ?? throw new ApplicationException("Frontend:BaseUrl configuration is missing.");

    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var user = await _databaseContext.Users
                .FirstOrDefaultAsync(u => u.NormalizedUsername == loginDto.Email.ToUpperInvariant());

            if (user == null || !PasswordHelper.VerifyHashedPassword(loginDto.Password, user.Password))
                return Unauthorized(new { message = "Invalid email or password.", isVerified = false });

            string token = TokenHelper.GenerateJwtToken(
                user.Uuid,
                user.NormalizedUsername,
                _signingCredentials,
                issuer: "ReTaskMe",
                audience: "ReTaskMeClients",
                expiresMinutes: 60
            );

            if (!user.IsVerified)
            {
                string verificationLink = $"{_frontendUrl}/api/v1/Email/verify-email?token={token}";

                string bodyContent = $@"
                    <p>Hi,</p>
                    <p>Please click the link below to verify your email:</p>
                    <p><a href='{verificationLink}'>Verify Email</a></p>
                    <p>If you did not register, ignore this email.</p>";

                string htmlContent = EmailTemplates.WelcomeTemplate(bodyContent);

                return Ok(new { isVerified = false, verificationToken = token, verificationLink });
            }

            return Ok(new 
            { 
                isVerified = true, 
                token = token,                
                expiresIn = 60 * 60           
            });
        }
        catch (Exception e)
        {
            return BadRequest(new { error = e.Message });
        }
    }
}


