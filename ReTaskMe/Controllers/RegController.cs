using Microsoft.AspNetCore.Mvc;
using BusinessLogicLayer.Services;
using BusinessLogicLayer.Services.Interfaces;
using BusinessLogicLayer.DTOs;
using System.Threading.Tasks;

namespace AuthBackend.Controllers
{
    [ApiController]
    [Route("api1/[controller]")]
    public class RegController : ControllerBase
    {

        private readonly IRegisterService _registerService;

        public RegController(IRegisterService registerService){
            _registerService = registerService;
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
