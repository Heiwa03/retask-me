using Microsoft.AspNetCore.Mvc;
using BusinessLogicLayerCore.Services.Interfaces;
using BusinessLogicLayerCore.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace ReTaskMe.Controllers;
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IUserService userService) : BaseController {
        private readonly IUserService _userService = userService;
        
        [HttpPost("createTask")]
        public async Task<IActionResult> ActionCreateTask([FromBody] TaskDTO dto){
        if (UserGuid.Value == Guid.Empty)
        {
            return NotFound(new { message = "UserGuid is NULL!!!" });
        }
            await _userService.CreateTask(dto, UserGuid.Value); // THIS
            return Ok(new { message = "Task created successfully" });
        }
        
        [HttpPost("updateTask/{taskUid:guid}")]
        public async Task<IActionResult> ActionUpdateTask([FromBody] TaskDTO dto, Guid taskUid){
            await _userService.UpdateTask(dto, UserGuid.Value, taskUid);
            return Ok(new { message = "Task updated successfully" });
        }
        
        [HttpDelete("deleteTask/{taskUid:guid}")]
        public async Task<IActionResult> ActionDeleteTask(Guid taskUid){
            await _userService.DeleteTask(UserGuid.Value, taskUid);
            return Ok(new { message = $"Task {taskUid} deleted successfully" });
        }
        
        [HttpGet("task/{taskUid:guid}")]
        public async Task<IActionResult> ActionGetTask(Guid taskUid){
            var task = await _userService.GetTask(UserGuid.Value, taskUid);

            if (task == null)
                return NotFound(new { message = $"Task {taskUid} not found" });

            return Ok(task); 
        }
    }

