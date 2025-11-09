using BusinessLogicLayerCore.Services;
using BusinessLogicLayerCore.Services.Interfaces;
using DataAccessLayerCore.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BusinessLogicLayerCore.Services.SearchBehaviour;


namespace ReTaskMe.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchTaskController : BaseController{
    public readonly SearchService _searchService;
    public readonly ITaskRepository _taskService;

    public SearchTaskController(SearchService _searchService, ITaskRepository _taskService){
        this._searchService = _searchService;
        this._taskService = _taskService;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string query, [FromQuery] string mode = "title"){
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest("Search query cannot be empty, bro...");

        switch (mode.ToLower()){
            case "title":
            default:
                _searchService.SetStrategy(new TitleSearchStrategy());
                break;
            // The rest strategy in future... maybe
        }

        var tasks = await _taskService.GetTasksByUserUidAsync(TestUserGuid ?? Guid.NewGuid());

        var result = _searchService.SearchTasks(tasks, query);

        return Ok(result);
    }
}