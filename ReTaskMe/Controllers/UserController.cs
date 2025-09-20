using Microsoft.AspNetCore.Mvc;
using BusinessLogicLayerCore.Services.Interfaces;
using BusinessLogicLayerCore.DTOs;
using System.Security.Claims;
using BusinessLogicLayerCore.Services;
using ReTaskMe.Models.Response;

namespace ReTaskMe.Controllers;
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IUserService userService) : ControllerBase{

        private readonly IUserService _userService = userService;

        [HttpPost("createTask")]
        public async Task<IActionResult> ActionCreateTask([FromBody] TaskDTO dto, [FromQuery] long userId){
            await _userService.CreateTask(dto, userId);

            return Ok(new { message = "Task created successfully" });
        }

        // TODO
        // [HttpPost("deleteTask")]

        // [HttpPost("updateTask")]

        // [HttpPost("sortTasksByStatus")]

        // [HttpPost("sortTasksByDeadline")]

        // [HttpPost("filterTasksByDeadline")]

    }

