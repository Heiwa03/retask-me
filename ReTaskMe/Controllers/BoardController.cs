using Microsoft.AspNetCore.Mvc;
using BusinessLogicLayerCore.Services.Interfaces;
using BusinessLogicLayerCore.DTOs;
using Microsoft.AspNetCore.Authorization;
using ReTaskMe.Models.Response;

namespace ReTaskMe.Controllers;
    [ApiController]
    [Route("api/[controller]")]
    public class BoardController(IUserService _userService) : BaseController {
    

        [HttpPost("createBoard")]
        public async Task<IActionResult> CreateBoard([FromBody] BoardDTO boardDto){
            await _userService.CreateBoard(boardDto, UserGuid ?? Guid.NewGuid());
            return Ok(new { message = "Board created successfully"});
        }

        [HttpPost("addTaskToBoard")]
        public async Task<IActionResult> AddTaskToBoard(Guid boardUuid, Guid taskUuid){
            await _userService.AddTaskToBoard(UserGuid ?? Guid.NewGuid(), boardUuid, taskUuid);
            return Ok(new { message = "Task added to board successfully"});
        }

        [HttpDelete("deleteTaskFromBoard")]
        public async Task<IActionResult> RemoveTaskFromBoard(Guid boardUuid, Guid taskUuid){
            await _userService.RemoveTaskFromBoard(UserGuid ?? Guid.NewGuid(), boardUuid, taskUuid);
            return Ok(new { message = "Task deleted successfully"});
        }

        [HttpGet("getBoardTask")]
        public async Task<IActionResult> GetUserBoards(){
            var board = await _userService.GetUserBoards(UserGuid ?? Guid.NewGuid());
            return Ok(board);
        }

        // [HttpGet("getBoardWithTask")]
        // public async Task<IActionResult> GetBoardWithTasks(Guid boardUuid){
        //     var board = await _userService.GetBoardWithTasks(TestUserGuid ?? Guid.NewGuid(), boardUuid);
        //     return Ok (board);
        // }

        [HttpGet("getTasksfromBoard")]
        public async Task<IActionResult> GetTasksFromBoard(Guid boardUuid){
            var board = await _userService.GetTasksFromBoard(UserGuid ?? Guid.NewGuid(), boardUuid);
            return Ok(board);
        }

    }

