using BusinessLogicLayerCore.DTOs;
using DataAccessLayerCore.Entities;

namespace BusinessLogicLayerCore.Services.Interfaces;

    public interface IUserService{
        // Tasks
        Task CreateTask(TaskDTO dto, Guid uuid);
        Task<DailyTask> GetTask(Guid uuid, Guid tuid);
        Task<List<DailyTask>> GetAllTasks(Guid uuid);
        Task UpdateTask(TaskDTO dto, Guid uuid, Guid tuid);
        Task DeleteTask(Guid uuid, Guid tuid);

        // Profile
        Task<PostRegisterDTO> GetUserProfile(Guid userUid);
        Task UpdateUserProfile(PostRegisterDTO dto, Guid userUid);

        // Board
        Task CreateBoard(BoardDTO boardDto, Guid userUuid);
        Task AddTaskToBoard(Guid userUuid, Guid boardUuid, Guid taskUuid);
        Task RemoveTaskFromBoard(Guid userUuid, Guid boardUuid, Guid taskUuid);
        Task<List<BoardDTO>> GetUserBoards(Guid userUuid);
        Task<List<TaskDTO>> GetTasksFromBoard(Guid userUuid, Guid boardUuid);
        //Task<BoardDTO> GetBoardWithTasks(Guid userUuid, Guid boardUuid);
    }