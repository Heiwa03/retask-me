using Microsoft.AspNetCore.Mvc;
using BusinessLogicLayerCore.Services.Interfaces;
using BusinessLogicLayerCore.DTOs;

namespace ReTaskMe.Controllers;
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IUserService userService) : BaseController {
        private readonly IUserService _userService = userService;

        [HttpPost("createTask")]
        public async Task<IActionResult> ActionCreateTask([FromBody] TaskDTO dto){
            await _userService.CreateTask(dto, UserGuid ?? new Guid("")); // THIS
            return Ok(new { message = "Task created successfully" });
        }

        [HttpPost("updateTask/{taskId:long}")]
        public async Task<IActionResult> ActionUpdateTask([FromBody] TaskDTO dto, Guid taskUid){
            await _userService.UpdateTask(dto, UserGuid ?? new Guid(""), taskUid);
            return Ok(new { message = "Task updated successfully" });
        }

        [HttpDelete("deleteTask/{taskId:long}")]
        public async Task<IActionResult> ActionDeleteTask(Guid taskUid){
            await _userService.DeleteTask(UserGuid ?? new Guid(""), taskUid);
            return Ok(new { message = $"Task {taskUid} deleted successfully" });
        }

        [HttpGet("task/{taskId:long}")]
        public async Task<IActionResult> ActionGetTask(Guid taskUid){
            var task = await _userService.GetTask(UserGuid ?? new Guid(""), taskUid);

            if (task == null)
                return NotFound(new { message = $"Task {taskUid} not found" });

            return Ok(task); 
        }
    }

