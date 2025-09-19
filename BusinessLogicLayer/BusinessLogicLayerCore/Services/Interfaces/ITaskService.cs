// BL
using BusinessLogicLayerCore.DTOs;

// DAL
using DataAccessLayerCore.Entities;

namespace BusinessLogicLayerCore.Services.Interfaces{
    public interface ITaskService{
        // 1. Main method for register
        Task CreateAndSaveTask(TaskDTO dto, long userId);
        Task<List<DailyTask>> GetAllTasks(long userId, long taskId);
        Task<DailyTask> GetTask(long userId, long taskId);
        Task DeleteTask(long userId, long taskId);
        Task UpdateTask(TaskDTO dto, long userId, long taskId);
    }
}