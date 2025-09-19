// BL
using BusinessLogicLayerCore.Services.Interfaces;
using BusinessLogicLayerCore.DTOs;

// DL
using DataAccessLayerCore.Entities;
using DataAccessLayerCore.Repositories.Interfaces;
using DataAccessLayerCore.Enum;


namespace BusinessLogicLayerCore.Services;
    public class TaskService(ITaskRepository _taskRepository) : ITaskService{

      // Create Task
      public async Task CreateAndSaveTask(TaskDTO dto, long userId) {
         var task = new DailyTask {
            Uuid = Guid.NewGuid(),
            TaskId = 1,
            UserId = userId,
            Title = dto.Title,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow,
            Deadline = dto.Deadline,
            Priority = dto.Priority,
            Status = StatusTask.InProgress
         };

          _taskRepository.Add(task);
          await _taskRepository.SaveChangesAsync();
      }

      // Reed one
      public async Task<DailyTask> GetTask(long userId, long taskId){
          var task = await _taskRepository.GetTaskByUserAndIdAsync(userId, taskId);

          if (task == null){
              throw new KeyNotFoundException($"Task {taskId} for user {userId} not found");
          }

          return task;
      }

      // Reed all
      public async Task<List<DailyTask>> GetAllTasks(long userId, long taskId){
          var tasks = await _taskRepository.GetTasksByUserIdAsync(userId);

          if (tasks == null || tasks.Count == 0){
              throw new KeyNotFoundException($"No tasks found for user {userId}");
          }

          return tasks;
      }

      //Delete tasks
      public async Task DeleteTask(long userId, long taskId){
          var task = await _taskRepository.GetTaskByUserAndIdAsync(userId, taskId);

          if (task == null){
              throw new KeyNotFoundException($"Task {taskId} for user {userId} not found");
          }

          _taskRepository.Delete(task);
          await _taskRepository.SaveChangesAsync();
      }

      // Update
      public async Task UpdateTask(TaskDTO dto, long userId, long taskId){
          var task = await _taskRepository.GetTaskByUserAndIdAsync(userId, taskId);

          if (task == null){
              throw new KeyNotFoundException($"Task {taskId} for user {userId} not found");
          }

          UpdateTaskForm(task, dto);
          _taskRepository.Update(task);
          await _taskRepository.SaveChangesAsync();
      }

      public void UpdateTaskForm(DailyTask task, TaskDTO dto){
          task.Title = dto.Title;
          task.Description = dto.Description;
          task.Deadline = dto.Deadline;
          task.Priority = dto.Priority;
          task.Status = dto.Status;
      }
    }

