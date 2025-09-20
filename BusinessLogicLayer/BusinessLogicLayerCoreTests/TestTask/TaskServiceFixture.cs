// System
using Xunit;
using Moq;

// BL
using BusinessLogicLayerCore.Services;
using BusinessLogicLayerCore.DTOs;

// DAL
using DataAccessLayerCore.Repositories.Interfaces;
using DataAccessLayerCore.Entities;
using DataAccessLayerCore.Enum;


namespace BusinessLogicLayerCoreTests.TestTask{
    public class TaskServiceFixture {

        private readonly Mock<ITaskRepository> _taskRepository = new();
        private readonly TaskService taskService;

        public TaskServiceFixture(){
            taskService = new TaskService(_taskRepository.Object);
        }
        // Variabiles for user and tasks ID
        public static class Globals{
            public static long userId = 100;
            public static long taskId = 1;
        }

        // TaskDTO test model
        public static TaskDTO Task(){
            TaskDTO task = new TaskDTO {
                Title = "Title",
                Description = "Desript",
                Deadline = null,
                Status = StatusTask.New,
                Priority = PriorityTask.High
            };

            return task;
        }

        // DailtyTask test model
        public static DailyTask DailyTask1(){
            DailyTask dailyTask = new DailyTask {
                Id = Globals.taskId,
                Uuid = Guid.NewGuid(),
                UserId = Globals.userId,
                Title = "Title",
                Description = "Desript",
                Status = StatusTask.New,
                Priority = PriorityTask.High
            };

            return dailyTask;
        }


        // TEST
        [Fact]
        public async Task CreateAndSaveTask_SucceedStatus() {
            long userId = 100;

            TaskDTO task = Task();

            _taskRepository
                .Setup(x => x.Add(It.Is<DailyTask>(t => t.Title == task.Title)))
                .Verifiable();

            await taskService.CreateAndSaveTask(task, userId);

            _taskRepository.Verify(r => r.Add(It.Is<DailyTask>(t =>
                t.Title == task.Title &&
                t.UserId == userId
            )), Times.Once);

            _taskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ReedOneTask_SuccedStatus(){
            DailyTask dailyTask = DailyTask1();

            _taskRepository
                .Setup(r => r.GetTaskByUserAndIdAsync(Globals.userId, Globals.taskId))
                .ReturnsAsync(dailyTask);

            // Act
            var result = await taskService.GetTask(Globals.userId, Globals.taskId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dailyTask.Id, result.Id);
            Assert.Equal(dailyTask.UserId, result.UserId);
            Assert.Equal(dailyTask.Title, result.Title);

            _taskRepository.Verify(r => r.GetTaskByUserAndIdAsync(Globals.userId, Globals.taskId), Times.Once);
        }

        [Fact]
        public async Task GetTask_ShouldThrow_WhenNotFound() {
            _taskRepository
                .Setup(r => r.GetTaskByUserAndIdAsync(Globals.userId, Globals.taskId))
                .ReturnsAsync((DailyTask?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => taskService.GetTask(Globals.userId, Globals.taskId)
            );
        }

        [Fact]
        public async Task GetAllTasks_SucceedStatus() {
            List<DailyTask> dailyTasks = new List<DailyTask> { DailyTask1() };

            _taskRepository
                .Setup(r => r.GetTasksByUserIdAsync(Globals.userId))
                .ReturnsAsync(dailyTasks);

            var result = await taskService.GetAllTasks(Globals.userId, Globals.taskId);

            Assert.NotEmpty(result);
            Assert.Equal(dailyTasks.Count, result.Count);

            _taskRepository.Verify(r => r.GetTasksByUserIdAsync(Globals.userId), Times.Once);
        }

        [Fact]
        public async Task GetAllTasks_ShouldThrow_WhenNoTasksFound() {
            _taskRepository
                .Setup(r => r.GetTasksByUserIdAsync(Globals.userId))
                .ReturnsAsync(new List<DailyTask>());

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => taskService.GetAllTasks(Globals.userId, Globals.taskId)
            );
        }

        [Fact]
        public async Task DeleteTask_SucceedStatus() {
            DailyTask dailyTask = DailyTask1();

            _taskRepository
                .Setup(r => r.GetTaskByUserAndIdAsync(Globals.userId, Globals.taskId))
                .ReturnsAsync(dailyTask);

            await taskService.DeleteTask(Globals.userId, Globals.taskId);

            _taskRepository.Verify(r => r.Delete(dailyTask), Times.Once);
            _taskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteTask_ShouldThrow_WhenNotFound() {
            _taskRepository
                .Setup(r => r.GetTaskByUserAndIdAsync(Globals.userId, Globals.taskId))
                .ReturnsAsync((DailyTask?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => taskService.DeleteTask(Globals.userId, Globals.taskId)
            );
        }

        [Fact]
        public async Task UpdateTask_SucceedStatus() {
            DailyTask dailyTask = DailyTask1();
            TaskDTO dto = Task();
            dto.Title = "Updated title";

            _taskRepository
                .Setup(r => r.GetTaskByUserAndIdAsync(Globals.userId, Globals.taskId))
                .ReturnsAsync(dailyTask);

            await taskService.UpdateTask(dto, Globals.userId, Globals.taskId);

            _taskRepository.Verify(r => r.Update(It.Is<DailyTask>(t =>
                t.Title == "Updated title"
            )), Times.Once);

            _taskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateTask_ShouldThrow_WhenNotFound() {
            TaskDTO dto = Task();

            _taskRepository
                .Setup(r => r.GetTaskByUserAndIdAsync(Globals.userId, Globals.taskId))
                .ReturnsAsync((DailyTask?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => taskService.UpdateTask(dto, Globals.userId, Globals.taskId)
            );
        }
    }
}




