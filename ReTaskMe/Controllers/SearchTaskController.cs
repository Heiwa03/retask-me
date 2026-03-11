using BusinessLogicLayerCore.Services;
using DataAccessLayerCore.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BusinessLogicLayerCore.Services.SearchBehaviour;
using ReTaskMe.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using BusinessLogicLayerCore.Services.Interfaces;


namespace ReTaskMe.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchTaskController : BaseController{
    public readonly ISearchService _searchService;
    public readonly ITaskRepository _taskService;

    public SearchTaskController(SearchService _searchService, ITaskRepository _taskService){
        this._searchService = _searchService;
        this._taskService = _taskService;
    }

    [Authorize]
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string query, [FromQuery] string mode = "title")
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest(new { Message = "Search query cannot be empty" });

        if (UserGuid is not Guid userGuid)
            return Unauthorized(new { Message = "User not authenticated" });

        switch (mode.ToLower())
        { 
            case "title":
                _searchService.SetStrategy(new TitleSearchStrategy());
                break;
            default:
                return BadRequest(new { Message = $"Unsupported search mode: {mode}" });
        }

        var filteredTasks = await _searchService.SearchTasks(userGuid, query);
        
        var taskModels = filteredTasks.Select(t => new TaskModel
        {
            Uuid = t.Uuid,
            Title = t.Title,
            Description = t.Description,
            Deadline = t.Deadline,
            Status = t.Status,
            Priority = t.Priority,
            BoardUuid = t.BoardUuid
        }).ToList();

        var response = new
        {
            Query = query,
            Mode = mode,
            TotalCount = taskModels.Count,
            Tasks = taskModels  
        };

        return Ok(response);
    }
}