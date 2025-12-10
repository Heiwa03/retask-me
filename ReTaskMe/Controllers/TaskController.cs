using Microsoft.AspNetCore.Mvc;
using BusinessLogicLayerCore.Services.Interfaces;
using BusinessLogicLayerCore.DTOs;
using Microsoft.AspNetCore.Authorization;
using ReTaskMe.Models.Response;

namespace ReTaskMe.Controllers;
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class TaskController(IUserService userService) : BaseController {
    private readonly IUserService _userService = userService;
    [HttpPost("createTask")]
    public async Task<IActionResult> ActionCreateTask([FromBody] TaskDTO dto){
        if (UserGuid is not Guid userGuid)
            return Unauthorized(new { message = "User not authenticated" });
        await _userService.CreateTask(dto, UserGuid.Value) ; // THIS
        return Ok(new { message = "Task created successfully" });
    }

    [HttpPost("updateTask/{taskUid:guid}")]
    public async Task<IActionResult> ActionUpdateTask([FromBody] TaskDTO dto, Guid taskUid){
        if (UserGuid is not Guid userGuid)
            return Unauthorized(new { message = "User not authenticated" });
        await _userService.UpdateTask(dto, UserGuid.Value, taskUid);
        return Ok(new { message = "Task updated successfully" });
    }

    [HttpDelete("deleteTask/{taskUid:guid}")]
    public async Task<IActionResult> ActionDeleteTask(Guid taskUid){
        if (UserGuid is not Guid userGuid)
            return Unauthorized(new { message = "User not authenticated" });
        await _userService.DeleteTask(UserGuid.Value, taskUid);
        return Ok(new { message = $"Task {taskUid} deleted successfully" });
    }

    [HttpGet("task/{taskUid:guid}")]
    public async Task<IActionResult> ActionGetTask(Guid taskUid){
        if (UserGuid is not Guid userGuid)
            return Unauthorized(new { message = "User not authenticated" });
        var task = await _userService.GetTask(UserGuid.Value, taskUid);

        if (task == null)
            return NotFound(new { message = $"Task {taskUid} not found" });

        var taskModel = new TaskModel{
            Title = task.Title,
            Description = task.Description,
            Deadline = task.Deadline,
            Priority = task.Priority,
            Status = task.Status
        };

        return Ok(taskModel); 
    }

}

