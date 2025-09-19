using Microsoft.AspNetCore.Mvc;
using BusinessLogicLayer.Services;
using BusinessLogicLayer.Services.Interfaces;
using BusinessLogicLayer.DTOs;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer;

namespace AuthBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        private readonly IRegisterService _registerService;

        private readonly DatabaseContext _databaseContext;

        public AuthController(IAuthService authService, IRegisterService registerService, DatabaseContext databaseContext)
        {
            _authService = authService;
            _registerService = registerService;
            _databaseContext = databaseContext;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var authResponse = await _authService.LoginAsync(loginDto.Email, loginDto.Password);

            if (authResponse == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            return Ok(authResponse);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO){
            
            try{
                await _registerService.RegisterUser(registerDTO);
                return Ok("User registred");

            } catch(Exception e){
                return BadRequest(e.Message);
            }
        }

        [HttpGet("db-check")]
        public async Task<IActionResult> DbCheck()
        {
            try
            {
                var canConnect = await _databaseContext.Database.CanConnectAsync();
                var userCount = await _databaseContext.Users.CountAsync();
                return Ok(new { canConnect, userCount });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { canConnect = false, error = ex.Message });
            }
        }

    }
}
