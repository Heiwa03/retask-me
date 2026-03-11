using BusinessLogicLayerCore.DTOs;

namespace BusinessLogicLayerCore.Services.Interfaces;

    public interface IBoardService{
        Task CreateBoard(BoardDTO boardDto, Guid userUuid);
        Task AddTaskToBoard(Guid userUuid, Guid boardUuid, Guid taskUuid);
        Task RemoveTaskFromBoard(Guid userUuid, Guid boardUuid, Guid taskUuid);
        Task DeleteBoard(Guid userUuid, Guid boardUuid);
        Task UpdateBoard(BoardDTO boardDTO, Guid userUuid, Guid boardUuid);


        Task<List<BoardDTO>> GetUserBoards(Guid userUuid);
        Task<List<TaskDTO>> GetTasksFromBoard(Guid userUuid, Guid boardUuid);

    }