// BL
using BusinessLogicLayerCore.DTOs;

// DAL
using DataAccessLayerCore.Entities;

namespace BusinessLogicLayerCore.Services.Interfaces{
    public interface ITaskService{
        // 1. Main method for register
        Task CreateAndSaveTask(TaskDTO dto, int userId);

        // Task UpdateTask();
        // Task DeleteTask();
        // Task GetTask();
    }
}