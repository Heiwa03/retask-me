
using BusinessLogicLayerCore.Services.AiAgentBehaviour;
using BusinessLogicLayerCore.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ReTaskMe.Controllers;

public class AiController : BaseController{
    private readonly AgentStrategyResolver _resolver;

    public AiController(AgentStrategyResolver _resolver){
        this._resolver = _resolver;
    }

    [HttpPost("AiAssist")]
    public async Task<IActionResult> GenerateBlyat([FromBody] String request){
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