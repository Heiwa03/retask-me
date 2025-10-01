

using BusinessLogicLayerCore.DTOs;
using BusinessLogicLayerCore.Services.Interfaces;
using DataAccessLayerCore.Entities;

namespace BusinessLogicLayerCore.Services;

    public class UserService : IUserService {
        private readonly ITaskService _taskService;
        private readonly IProfileService _profileService;

        public UserService(ITaskService taskService, IProfileService profileService)
        {
            _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
            _profileService = profileService ?? throw new ArgumentNullException(nameof(profileService));
        }
        
        // Delegete to taskServ
        public async Task CreateTask(TaskDTO dto, Guid userUid){
            await _taskService.CreateAndSaveTask(dto, userUid);
        }

        public async Task<DailyTask> GetTask(Guid userUid, Guid taskUid){
            return await _taskService.GetTask(userUid, taskUid);
        }

        public async Task<List<DailyTask>> GetAllTasks(Guid userUid){
            return await _taskService.GetAllTasks(userUid);
        }

        public async Task UpdateTask(TaskDTO dto, Guid userUid, Guid taskUid){
            await _taskService.UpdateTask(dto, userUid, taskUid);
        }

        public async Task DeleteTask(Guid userUid, Guid taskUid){
            await _taskService.DeleteTask(userUid, taskUid);
        }

        // Delegete to profServ
        public async Task<PostRegisterDTO> GetUserProfile(Guid userUid){
            return await _profileService.GetProfile(userUid);
        }

        public async Task UpdateUserProfile(PostRegisterDTO dto, Guid userUid){
            await _profileService.UpdateProfile(dto, userUid);
        }
    }
    
        

