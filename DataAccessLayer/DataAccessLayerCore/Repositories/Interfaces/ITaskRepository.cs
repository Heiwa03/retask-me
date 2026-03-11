
using DataAccessLayerCore.Enum;
using DataAccessLayerCore.Entities;


namespace DataAccessLayerCore.Repositories.Interfaces{
    public interface ITaskRepository : IBaseRepository{
        Task<List<DailyTask>> GetTasksByUserUidAsync(Guid uuid);
        Task<DailyTask?> GetTaskByUserUidAsync(Guid uuid, Guid tuid);
        //Task<List<DailyTask>> GetTasksByStatusAsync(long userId, StatusTask status);
        //Task<List<TaskModel>> GetTasksByBoardIdAsync(int boardId); Temporaty closed
    }
}