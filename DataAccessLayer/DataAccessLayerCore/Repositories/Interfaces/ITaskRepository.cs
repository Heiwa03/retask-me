
using DataAccessLayerCore.Enum;
using DataAccessLayerCore.Entities;


namespace DataAccessLayerCore.Repositories.Interfaces{
    public interface ITaskRepository{
        Task<List<TaskModel>> GetTasksByUserIdAsync(int userId);
        Task<List<TaskModel>> GetTasksByStatusAsync(int userId, StatusTask status);
        //Task<List<TaskModel>> GetTasksByBoardIdAsync(int boardId); Temporaty closed
    }
}