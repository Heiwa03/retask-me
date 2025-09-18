// BL
using BusinessLogicLayerCore.Services.Interfaces;
using BusinessLogicLayerCore.DTOs;

// DL
using DataAccessLayerCore.Entities;
using DataAccessLayerCore.Repositories.Interfaces;
using DataAccessLayerCore.Enum;

namespace BusinessLogicLayerCore.Services{
    public class TaskService(ITaskRepository taskRepository) : ITaskService{


      // Create Task
      public async Task CreateAndSaveTask(TaskDTO dto, int userId) {
         var task = new DailyTask {
            Uuid = Guid.NewGuid(),
            TaskId = 0,
            UserId = userId,
            Title = dto.Title,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow,
            Deadline = dto.Deadline,
            Priority = dto.Priority,
            Status = StatusTask.InProgress
            //BoardId = dto.BoardId,
         };

          taskRepository.Add(task);
          await taskRepository.SaveChangesAsync();
       }


    }
}
