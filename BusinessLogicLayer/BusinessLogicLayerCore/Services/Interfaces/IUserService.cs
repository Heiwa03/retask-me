using BusinessLogicLayerCore.DTOs;
using DataAccessLayerCore.Entities;

namespace BusinessLogicLayerCore.Services.Interfaces;

    public interface IUserService{
        Task CreateTask(TaskDTO dto, Guid uuid);
        Task<DailyTask> GetTask(Guid uuid, Guid tuid);
        Task<List<DailyTask>> GetAllTasks(Guid uuid);
        Task UpdateTask(TaskDTO dto, Guid uuid, Guid tuid);
        Task DeleteTask(Guid uuid, Guid tuid);

        Task<PostRegisterDTO> GetUserProfile(Guid userUid);
        Task UpdateUserProfile(PostRegisterDTO dto, Guid userUid);
    }