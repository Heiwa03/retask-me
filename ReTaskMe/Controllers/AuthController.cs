using Microsoft.AspNetCore.Mvc;
using BusinessLogicLayerCore.Services;
using BusinessLogicLayerCore.Services.Interfaces;
using BusinessLogicLayerCore.DTOs;
using System.Threading.Tasks;

namespace AuthBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        private readonly IRegisterService _registerService;

        public AuthController(IAuthService authService, IRegisterService registerService)
        {
            _authService = authService;
            _registerService = registerService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var authResponse = await _authService.LoginAsync(loginDto.Username, loginDto.Password);

            if (authResponse == null)
            {
                return Unauthorized("Invalid username or password.");
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

    }
}
