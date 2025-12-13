
using BusinessLogicLayerCore.Services.AiAgentBehaviour;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ReTaskMe.Controllers;

public class AiController : BaseController{
    private readonly AgentStrategyResolver _resolver;

    public AiController(AgentStrategyResolver _resolver){
        this._resolver = _resolver;
    }

    [Authorize]
    [HttpPost("AiAssist")]
    public async Task<IActionResult> GenerateContent([FromBody] String request){
        if (!UserGuid.HasValue)
        {
            return BadRequest(new { error = "User is not authenticated or UserGuid is missing." });
        }

        try{
            var strategy = _resolver.Resolve(request);

            var result = await strategy.HandleAsync(request, UserGuid.Value);

            return Ok(new { result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}