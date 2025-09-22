// BL
using BusinessLogicLayerCore.Services.Interfaces;
using BusinessLogicLayerCore.DTOs;

// DL
using DataAccessLayerCore.Entities;
using DataAccessLayerCore.Repositories.Interfaces;


namespace BusinessLogicLayerCore.Services;
    public class TaskService(ITaskRepository _taskRepository) : ITaskService{

      // Create Task
      public async Task CreateAndSaveTask(TaskDTO dto, Guid uuid) {
         var task = new DailyTask {
            Uuid = Guid.NewGuid(),
            TaskUid = Guid.NewGuid(),
            UserUid = uuid,
            Title = dto.Title,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow,
            Deadline = dto.Deadline,
            Priority = dto.Priority,
            Status = dto.Status
         };

          _taskRepository.Add(task);
          await _taskRepository.SaveChangesAsync();
      }

      // Reed one
      public async Task<DailyTask> GetTask(Guid uuid, Guid tuid){
          var task = await _taskRepository.GetTaskByUserUidAsync(uuid, tuid);

          if (task == null){
              throw new KeyNotFoundException($"Task {tuid} for user {uuid} not found");
          }

          return task;
      }

      // Reed all
      public async Task<List<DailyTask>> GetAllTasks(Guid uuid, Guid tuid){
          var tasks = await _taskRepository.GetTasksByUserUidAsync(uuid);

          if (tasks == null || tasks.Count == 0){
              throw new KeyNotFoundException($"No tasks found for user {uuid}");
          }

          return tasks;
      }

      //Delete task
      public async Task DeleteTask(Guid uuid, Guid tuid){
          var task = await _taskRepository.GetTaskByUserUidAsync(uuid, tuid);

          if (task == null){
              throw new KeyNotFoundException($"Task {tuid} for user {uuid} not found");
          }

          _taskRepository.Delete(task);
          await _taskRepository.SaveChangesAsync();
      }

      // Update
      public async Task UpdateTask(TaskDTO dto, Guid uuid, Guid tuid){
          var task = await _taskRepository.GetTaskByUserUidAsync(uuid, tuid);

          if (task == null){
              throw new KeyNotFoundException($"Task {tuid} for user {uuid} not found");
          }

          UpdateTaskForm(task, dto);
          _taskRepository.Update(task);
          await _taskRepository.SaveChangesAsync();
      }

      private static void UpdateTaskForm(DailyTask task, TaskDTO dto){
          task.Title = dto.Title;
          task.Description = dto.Description;
          task.Deadline = dto.Deadline;
          task.Priority = dto.Priority;
          task.Status = dto.Status;
      }
    }

