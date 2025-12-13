using BusinessLogicLayerCore.DTOs;
using DataAccessLayerCore.Entities;

namespace BusinessLogicLayerCore.Services.Interfaces;

    public interface IBoardService{
        Task CreateBoard(BoardDTO boardDto, Guid userUuid);
        Task AddTaskToBoard(Guid userUuid, Guid boardUuid, Guid taskUuid);
        Task RemoveTaskFromBoard(Guid userUuid, Guid boardUuid, Guid taskUuid);

        Task<List<BoardDTO>> GetUserBoards(Guid userUuid);
        Task<List<TaskDTO>> GetTasksFromBoard(Guid userUuid, Guid boardUuid);
        //Task<BoardDTO> GetBoardWithTasks(Guid userUuid, Guid boardUuid);

    }