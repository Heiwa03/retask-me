using Microsoft.AspNetCore.Mvc;
using BusinessLogicLayerCore.Services.Interfaces;
using BusinessLogicLayerCore.DTOs;
using System.Threading.Tasks;

namespace AuthBackend.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class RegController : ControllerBase
    {
        private readonly IRegisterService _registerService;

        public RegController(IRegisterService registerService)
        {
            _registerService = registerService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _registerService.RegisterUser(registerDTO);
                return Ok(new { Message = "User registered. Verification email sent." });
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }
    }
}
