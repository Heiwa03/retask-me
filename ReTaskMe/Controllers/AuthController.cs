
﻿using Microsoft.AspNetCore.Mvc;
using BusinessLogicLayerCore.Services.Interfaces;
using BusinessLogicLayerCore.DTOs;

using ReTaskMe.Models.Responses;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly string? _frontendUrl;
    private readonly string? _issuer;
    private readonly string? _audience;

    public AuthController(
        IAuthService authService,
        IConfiguration configuration)
    {
        _authService = authService;
        _frontendUrl = configuration["Frontend:BaseUrl"];
        _issuer = configuration["Jwt:Issuer"];
        _audience = configuration["Jwt:Audience"];
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto){
        var response = await _authService.LoginAsync(loginDto);
        
        return Ok( new AuthModel
        {
            token = response.Token,
            refreshToken = response.RefreshToken,
            message = "[*] Welcome!",
            isVerified = true
        });
    }    

    [HttpGet("refresh_token")]
    public async Task<IActionResult> RefreshToken(string currentRefreshToken){
        var token = await _authService.RefreshAsync(currentRefreshToken);

        return Ok(new AuthModel
        {
            token = token?.Token,
            refreshToken = token?.RefreshToken, 
            isVerified = true,
            message = "[*] Token refreshed"
        });
    }
}

