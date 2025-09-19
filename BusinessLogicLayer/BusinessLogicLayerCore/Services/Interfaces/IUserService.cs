using BusinessLogicLayerCore.DTOs;

namespace BusinessLogicLayerCore.Services.Interfaces;

    public interface IUserService{
        Task CreateTask(TaskDTO dto, long userId);
        Task GetTask(long userId, long taskId);
        Task GetAllTasks(long userId, long taskID);
        Task UpdateTask(TaskDTO dto, long userId, long taskId);
        Task DeleteTask(long userId, long taskId);
    }