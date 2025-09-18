
using DataAccessLayerCore.Enum;
using DataAccessLayerCore.Entities;


namespace DataAccessLayerCore.Repositories.Interfaces{
    public interface ITaskRepository : IBaseRepository{
        Task<List<DailyTask>> GetTasksByUserIdAsync(int userId);
        Task<List<DailyTask>> GetTasksByStatusAsync(int userId, StatusTask status);
        //Task<List<TaskModel>> GetTasksByBoardIdAsync(int boardId); Temporaty closed
    }
}