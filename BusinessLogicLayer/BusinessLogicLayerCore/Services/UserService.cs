

using BusinessLogicLayerCore.DTOs;
using BusinessLogicLayerCore.Services.Interfaces;

namespace BusinessLogicLayerCore.Services;

    public class UserService(ITaskService _taskService) : IUserService {
        public async Task CreateTask(TaskDTO dto, long userId){
            await _taskService.CreateAndSaveTask(dto, userId);
        }

        public async Task GetTask(long userId, long taskId){
            await _taskService.GetTask(userId, taskId);
        }

        public async Task GetAllTasks(long userId, long taskId){
            await _taskService.GetAllTasks(userId, taskId);
        }

        public async Task UpdateTask(TaskDTO dto, long userId, long taskId){
            await _taskService.UpdateTask(dto, userId, taskId);
        }

        public async Task DeleteTask(long userId, long taskId){
            await _taskService.DeleteTask(userId, taskId);
        }
    }
    
        

