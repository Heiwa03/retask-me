// BL
using BusinessLogicLayerCore.DTOs;

// DAL
using DataAccessLayerCore.Entities;

namespace BusinessLogicLayerCore.Services.Interfaces{
    public interface ITaskService{
        // 1. Main method for register
        Task CreateAndSaveTask(TaskDTO dto, Guid uuid);
        Task<List<DailyTask>> GetAllTasks(Guid uuid, Guid tuid);
        Task<DailyTask> GetTask(Guid uuid, Guid tuid);
        Task DeleteTask(Guid uuid, Guid tuid);
        Task UpdateTask(TaskDTO dto, Guid uuid, Guid tuid);
    }
}