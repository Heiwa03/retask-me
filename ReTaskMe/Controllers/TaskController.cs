using Microsoft.AspNetCore.Mvc;
using BusinessLogicLayerCore.Services.Interfaces;
using BusinessLogicLayerCore.DTOs;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Authorization;
using ReTaskMe.Models.Response;

namespace ReTaskMe.Controllers;
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController(IUserService userService) : BaseController {
        private readonly IUserService _userService = userService;
        [HttpPost("createTask")]
        public async Task<IActionResult> ActionCreateTask([FromBody] TaskDTO dto){
            await _userService.CreateTask(dto, TestUserGuid ?? Guid.NewGuid()) ; // THIS
            return Ok(new { message = "Task created successfully" });
        }

        [HttpPost("updateTask/{taskUid:guid}")]
        public async Task<IActionResult> ActionUpdateTask([FromBody] TaskDTO dto, Guid taskUid){
            await _userService.UpdateTask(dto, TestUserGuid ?? new Guid(), taskUid);
            return Ok(new { message = "Task updated successfully" });
        }

        [HttpDelete("deleteTask/{taskUid:guid}")]
        public async Task<IActionResult> ActionDeleteTask(Guid taskUid){
            await _userService.DeleteTask(TestUserGuid ?? new Guid(), taskUid);
            return Ok(new { message = $"Task {taskUid} deleted successfully" });
        }

        [HttpGet("task/{taskUid:guid}")]
        public async Task<IActionResult> ActionGetTask(Guid taskUid){
            var task = await _userService.GetTask(TestUserGuid ?? new Guid(), taskUid);

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

