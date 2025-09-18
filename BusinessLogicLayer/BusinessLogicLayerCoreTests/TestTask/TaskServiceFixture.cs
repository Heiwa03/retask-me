// System
using Xunit;
using Moq;

// BL
using BusinessLogicLayerCore.Services;

// DAL
using DataAccessLayerCore.Repositories.Interfaces;
using DataAccessLayerCore.Entities;
using DataAccessLayerCore.Enum;
using BusinessLogicLayerCore.DTOs;


namespace BusinessLogicLayerCoreTests.TestTask;

    public class TaskServiceFixture {

    private Mock<ITaskRepository> _taskRepository = new();

    private readonly TaskService taskService;

    [Fact]
    public async Task CreateAndSaveTask_SucceedStatus() {
        int userId = 100;

        var task = new TaskDTO {
            Title = "Title",
            Description = "Desript",
            Deadline = null,
            Status = StatusTask.New,
            Priority = PriorityTask.High
        };

        _taskRepository.Setup(x => x.Add(It.Is<DailyTask>(t => t.Title == task.Title))).Verifiable();


        await taskService.CreateAndSaveTask(task, userId);

        _taskRepository.Verify(r => r.Add(It.Is<DailyTask>(t => 
            t.Title == task.Title &&
            t.UserId == userId
        )), Times.Once);


        _taskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}


