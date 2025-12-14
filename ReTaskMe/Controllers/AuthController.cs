
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
    private readonly string _issuer;
    private readonly string _audience;

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
        _issuer = configuration["Jwt:Issuer"] ?? throw new ApplicationException("Jwt:Issuer is missing.");
        _audience = configuration["Jwt:Audience"] ?? throw new ApplicationException("Jwt:Audience is missing.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var user = await _databaseContext.Users
                .FirstOrDefaultAsync(u => u.NormalizedUsername == loginDto.Email.ToUpperInvariant());

            //var user = await _authService.LoginAsync(loginDto.Email, loginDto.Password);

            if (user == null || !PasswordHelper.VerifyHashedPassword(loginDto.Password, user.Password))
                return Unauthorized(new { message = "Invalid email or password." });

            // Skip verification check, treat user as verified
            user.IsVerified = true; // Optional: set this if you want to store it

            // Generate JWT token
            string accessToken = TokenHelper.GenerateJwtToken(
                user.Uuid,
                user.NormalizedUsername,
                _signingCredentials,
                issuer: _issuer,
                audience: _audience,
                expiresMinutes: 60
            );

            // Optional: generate refresh token
            string refreshToken = TokenHelper.GenerateRefreshToken();

            return Ok(new
            {
                message = "Login successful.",
                isVerified = true,
                token = accessToken,
                refreshToken = refreshToken
            });
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}

