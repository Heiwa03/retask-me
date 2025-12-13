using Microsoft.AspNetCore.Mvc;
using BusinessLogicLayerCore.Services.Interfaces;
using BusinessLogicLayerCore.DTOs;
using ReTaskMe.Models.Responses;

namespace ReTaskMe.Controllers;
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController(ITaskService _taskService) : BaseController {
        private readonly ITaskService _taskService = _taskService;
        [HttpPost("createTask")]
        public async Task<IActionResult> ActionCreateTask([FromBody] TaskDTO dto){
            await _taskService.CreateAndSaveTask(dto, UserGuid ?? Guid.NewGuid()) ; // !!!THIS!!!!
            return Ok(new { message = "Task created successfully" });
        }

        [HttpPost("updateTask/{taskUid:guid}")]
        public async Task<IActionResult> ActionUpdateTask([FromBody] TaskDTO dto, Guid taskUid){
            await _taskService.UpdateTask(dto, UserGuid ?? new Guid(), taskUid);
            return Ok(new { message = "Task updated successfully" });
        }

        [HttpDelete("deleteTask/{taskUid:guid}")]
        public async Task<IActionResult> ActionDeleteTask(Guid taskUid){
            await _taskService.DeleteTask(UserGuid ?? new Guid(), taskUid);
            return Ok(new { message = $"Task {taskUid} deleted successfully" });
        }

        [HttpGet("task/{taskUid:guid}")]
        public async Task<IActionResult> ActionGetTask(Guid taskUid){
            var task = await _taskService.GetTask(UserGuid ?? new Guid(), taskUid);

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

