
using DataAccessLayerCore.Enum;
using DataAccessLayerCore.Entities;


namespace DataAccessLayerCore.Repositories.Interfaces{
    public interface ITaskRepository : IBaseRepository{
        Task<List<DailyTask>> GetTasksByUserIdAsync(long userId);
        Task<DailyTask?> GetTaskByUserAndIdAsync(long userId, long taskId);
        //Task<List<DailyTask>> GetTasksByStatusAsync(long userId, StatusTask status);
        //Task<List<TaskModel>> GetTasksByBoardIdAsync(int boardId); Temporaty closed
    }
}