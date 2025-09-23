

using BusinessLogicLayerCore.DTOs;
using BusinessLogicLayerCore.Services.Interfaces;
using DataAccessLayerCore.Entities;

namespace BusinessLogicLayerCore.Services;

    public class UserService(ITaskService _taskService) : IUserService {

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

        // public async Task<RegisterDTO> GetUserProfile(){
        //     var profile = 
        // }
    }
    
        

