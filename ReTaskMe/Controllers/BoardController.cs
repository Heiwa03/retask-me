using Microsoft.AspNetCore.Mvc;
using BusinessLogicLayerCore.Services.Interfaces;
using BusinessLogicLayerCore.DTOs;
using Microsoft.AspNetCore.Authorization;





namespace ReTaskMe.Controllers;
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BoardController(IBoardService _boardService) : BaseController {
    
        [Authorize]
        [HttpPost("createBoard")]
        public async Task<IActionResult> CreateBoard([FromBody] BoardDTO boardDto){
            if (UserGuid is not Guid userGuid)
            return Unauthorized(new { message = "User not authenticated" });

            await _boardService.CreateBoard(boardDto, UserGuid.Value);
            return Ok(new { message = "Board created successfully"});
        }

        [Authorize]
        [HttpPost("addTaskToBoard")]
        public async Task<IActionResult> AddTaskToBoard(Guid boardUuid, Guid taskUuid){
            if (UserGuid is not Guid userGuid)
            return Unauthorized(new { message = "User not authenticated" });

            await _boardService.AddTaskToBoard(UserGuid.Value, boardUuid, taskUuid);
            return Ok(new { message = "Task added to board successfully"});
        }

        [Authorize]
        [HttpDelete("deleteTaskFromBoard")]
        public async Task<IActionResult> RemoveTaskFromBoard(Guid boardUuid, Guid taskUuid){
            if (UserGuid is not Guid userGuid)
            return Unauthorized(new { message = "User not authenticated" });

            await _boardService.RemoveTaskFromBoard(UserGuid.Value, boardUuid, taskUuid);
            return Ok(new { message = "Task deleted successfully"});
        }

        [Authorize]
        [HttpGet("getBoardTask")]
        public async Task<IActionResult> GetUserBoards(){
            if (UserGuid is not Guid userGuid)
            return Unauthorized(new { message = "User not authenticated" });

            var board = await _boardService.GetUserBoards(UserGuid.Value);
            return Ok(board);
        }

        [Authorize]
        [HttpGet("getTasksfromBoard")]
        public async Task<IActionResult> GetTasksFromBoard(Guid boardUuid){
            if (UserGuid is not Guid userGuid)
                return Unauthorized(new { message = "User not authenticated" });

            var board = await _boardService.GetTasksFromBoard(UserGuid.Value, boardUuid);
            return Ok(board);
        }

        [Authorize]
        [HttpPut("updateBoard")]
        public async Task<IActionResult> UpdateBoard([FromBody] BoardDTO boardDTO, Guid boardUuid){
            if (UserGuid is not Guid userGuid)
            return Unauthorized(new { message = "User not authenticated" });

            var board = _boardService.UpdateBoard(boardDTO, UserGuid.Value, boardUuid);
            return Ok(board);
        }

        [Authorize]
        [HttpDelete("deleteBoard")]
        public async Task<IActionResult> DeleteBoard(Guid boardUuid){
            if (UserGuid is not Guid userGuid)
            return Unauthorized(new { message = "User not authenticated" });

            var board = _boardService.DeleteBoard(boardUuid, UserGuid.Value);
            return Ok(board);
        }
    }

