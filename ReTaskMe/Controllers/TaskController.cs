using Microsoft.AspNetCore.Mvc;
using BusinessLogicLayerCore.Services.Interfaces;
using BusinessLogicLayerCore.DTOs;
using Microsoft.AspNetCore.Authorization;
using ReTaskMe.Models.Responses;
using System.Linq;

namespace ReTaskMe.Controllers;
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class TaskController(ITaskService _taskService) : BaseController {
    private readonly ITaskService _taskService = _taskService;

    [Authorize]
    [HttpPost("createTask")]
    public async Task<IActionResult> ActionCreateTask([FromBody] TaskDTO dto){
        if (UserGuid is not Guid userGuid)
            return Unauthorized(new { message = "User not authenticated" });
        await _taskService.CreateAndSaveTask(dto, UserGuid.Value) ; // THIS
        return Ok(new { message = "Task created successfully" });
    }

    [Authorize]
    [HttpPut("updateTask/{taskUid:guid}")]
    public async Task<IActionResult> ActionUpdateTask([FromBody] TaskDTO dto, Guid taskUid){
        if (UserGuid is not Guid userGuid)
            return Unauthorized(new { message = "User not authenticated" });
        await _taskService.UpdateTask(dto, UserGuid.Value, taskUid);
        return Ok(new { message = "Task updated successfully" });
    }

    [Authorize]
    [HttpDelete("deleteTask/{taskUid:guid}")]
    public async Task<IActionResult> ActionDeleteTask(Guid taskUid){
        if (UserGuid is not Guid userGuid)
            return Unauthorized(new { message = "User not authenticated" });
        await _taskService.DeleteTask(UserGuid.Value, taskUid);
        return Ok(new { message = $"Task {taskUid} deleted successfully" });
    }

    [Authorize]
    [HttpGet("task/{taskUid:guid}")]
    public async Task<IActionResult> ActionGetTask(Guid taskUid){
        if (UserGuid is not Guid userGuid)
            return Unauthorized(new { message = "User not authenticated" });
        var task = await _taskService.GetTask(UserGuid.Value, taskUid);

        if (task == null)
            return NotFound(new { message = $"Task {taskUid} not found" });

        var taskModel = new TaskModel{
            Uuid = task.Uuid,
            Title = task.Title,
            Description = task.Description,
            Deadline = task.Deadline,
            Priority = task.Priority,
            Status = task.Status
        };

        return Ok(taskModel); 
    }

    [Authorize]
    [HttpGet("tasks")]
    public async Task<IActionResult> ActionGetTasks(){
        if (UserGuid is not Guid userGuid)
            return Unauthorized(new { message = "User not authenticated" });

        var tasks = await _taskService.GetAllTasks(userGuid);

        var taskModels = tasks.Select(task => new TaskModel{
            Uuid = task.Uuid,
            Title = task.Title,
            Description = task.Description,
            Deadline = task.Deadline,
            Priority = task.Priority,
            Status = task.Status
        }).ToList();

        return Ok(taskModels);
    }

}

