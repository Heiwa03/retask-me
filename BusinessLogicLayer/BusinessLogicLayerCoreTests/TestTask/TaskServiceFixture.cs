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
            public static Guid userUid = Guid.NewGuid();
            public static Guid taskUid = Guid.NewGuid();
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
                TaskUid = Globals.taskUid,
                Uuid = Guid.NewGuid(),
                UserUid= Globals.userUid,
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
            Guid userUid = Guid.NewGuid();

            TaskDTO task = Task();

            _taskRepository
                .Setup(x => x.Add(It.Is<DailyTask>(t => t.Title == task.Title)))
                .Verifiable();

            await taskService.CreateAndSaveTask(task, userUid);

            _taskRepository.Verify(r => r.Add(It.Is<DailyTask>(t =>
                t.Title == task.Title &&
                t.UserUid == userUid
            )), Times.Once);

            _taskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ReedOneTask_SuccedStatus(){
            DailyTask dailyTask = DailyTask1();

            _taskRepository
                .Setup(r => r.GetTaskByUserUidAsync(Globals.userUid, Globals.taskUid))
                .ReturnsAsync(dailyTask);

            // Act
            var result = await taskService.GetTask(Globals.userUid, Globals.taskUid);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dailyTask.Id, result.Id);
            Assert.Equal(dailyTask.UserUid, result.UserUid);
            Assert.Equal(dailyTask.Title, result.Title);

            _taskRepository.Verify(r => r.GetTaskByUserUidAsync(Globals.userUid, Globals.taskUid), Times.Once);
        }

        [Fact]
        public async Task GetTask_ShouldThrow_WhenNotFound() {
            _taskRepository
                .Setup(r => r.GetTaskByUserUidAsync(Globals.userUid, Globals.taskUid))
                .ReturnsAsync((DailyTask?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => taskService.GetTask(Globals.userUid, Globals.userUid)
            );
        }

        [Fact]
        public async Task GetAllTasks_SucceedStatus() {
            List<DailyTask> dailyTasks = new List<DailyTask> { DailyTask1() };

            _taskRepository
                .Setup(r => r.GetTasksByUserUidAsync(Globals.userUid))
                .ReturnsAsync(dailyTasks);

            var result = await taskService.GetAllTasks(Globals.userUid, Globals.taskUid);

            Assert.NotEmpty(result);
            Assert.Equal(dailyTasks.Count, result.Count);

            _taskRepository.Verify(r => r.GetTasksByUserUidAsync(Globals.userUid), Times.Once);
        }

        [Fact]
        public async Task GetAllTasks_ShouldThrow_WhenNoTasksFound() {
            _taskRepository
                .Setup(r => r.GetTasksByUserUidAsync(Globals.userUid))
                .ReturnsAsync(new List<DailyTask>());

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => taskService.GetAllTasks(Globals.userUid, Globals.taskUid)
            );
        }

        [Fact]
        public async Task DeleteTask_SucceedStatus() {
            DailyTask dailyTask = DailyTask1();

            _taskRepository
                .Setup(r => r.GetTaskByUserUidAsync(Globals.userUid, Globals.taskUid))
                .ReturnsAsync(dailyTask);

            await taskService.DeleteTask(Globals.userUid, Globals.taskUid);

            _taskRepository.Verify(r => r.Delete(dailyTask), Times.Once);
            _taskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteTask_ShouldThrow_WhenNotFound() {
            _taskRepository
                .Setup(r => r.GetTaskByUserUidAsync(Globals.userUid, Globals.taskUid))
                .ReturnsAsync((DailyTask?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => taskService.DeleteTask(Globals.userUid, Globals.taskUid)
            );
        }

        [Fact]
        public async Task UpdateTask_SucceedStatus() {
            DailyTask dailyTask = DailyTask1();
            TaskDTO dto = Task();
            dto.Title = "Updated title";

            _taskRepository
                .Setup(r => r.GetTaskByUserUidAsync(Globals.userUid, Globals.taskUid))
                .ReturnsAsync(dailyTask);

            await taskService.UpdateTask(dto, Globals.userUid, Globals.taskUid);

            _taskRepository.Verify(r => r.Update(It.Is<DailyTask>(t =>
                t.Title == "Updated title"
            )), Times.Once);

            _taskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateTask_ShouldThrow_WhenNotFound() {
            TaskDTO dto = Task();

            _taskRepository
                .Setup(r => r.GetTaskByUserUidAsync(Globals.userUid, Globals.taskUid))
                .ReturnsAsync((DailyTask?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => taskService.UpdateTask(dto, Globals.userUid, Globals.taskUid)
            );
        }
    }
}




