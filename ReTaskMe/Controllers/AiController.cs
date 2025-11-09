
using BusinessLogicLayerCore.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ReTaskMe.Controllers;

public class AiController : BaseController{
    private readonly IAgentService _aiService;

    public AiController(IAgentService _aiService){
        this._aiService = _aiService;
    }

    public class AiPromtRequest{
        public string Prompt { get; set; } = string.Empty;
    }

    // [HttpPost("AiAssist")]
    // public async Task<IActionResult> GenerateBlyat([FromBody] AiPromtRequest request){
    //     if (string.IsNullOrWhiteSpace(request.Prompt)){
    //         return BadRequest("Prompt cannot be empty.");
    //     }

    //     var response = await _aiService.GetAiResponse(request.Prompt);
    //     return Ok(new { response });
    // }
}