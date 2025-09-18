// BL
using BusinessLogicLayerCore.Services.Interfaces;
using BusinessLogicLayerCore.DTOs;

// DL
using DataAccessLayerCore.Entities;

namespace BusinessLogicLayerCore.Services{
    public class TaskService : ITaskService{
      
      // Create Task
       public async Task<TaskModel> CreateTask(TaskDTO dto, int userId){
         TaskModel task = new TaskModel{
            UserId = userId,
            Title = dto.Title,
            Description = dto.Description,
            Deadline = dto.Deadline,
            Priority = dto.Priority
            //BoardId = dto.BoardId,
         };
         return task;
       }


    }
}
