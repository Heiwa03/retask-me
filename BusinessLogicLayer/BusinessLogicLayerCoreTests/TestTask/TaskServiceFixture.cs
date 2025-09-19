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
    }
}




