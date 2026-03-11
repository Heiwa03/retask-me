// System
using Xunit;
using Moq;
using Xunit.Abstractions;

// BL
using BusinessLogicLayerCore.Services;
using BusinessLogicLayerCore.DTOs;

// DAL
using DataAccessLayerCore.Repositories.Interfaces;
using DataAccessLayerCore.Entities;
using DataAccessLayerCore.Enum;

namespace BusinessLogicLayerCoreTests.TestTask;
    
public class TaskServiceFixture
{
    private readonly Mock<ITaskRepository> _taskRepository = new();
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly TaskService _taskService;
    private readonly ITestOutputHelper _output;

    public TaskServiceFixture(ITestOutputHelper output)
    {
        _taskService = new TaskService(_taskRepository.Object, _userRepository.Object);
        _output = output;
    }

    // Тестовый пользователь
    public static User TestUser => new User
    {
        Id = 123,
        Uuid = Guid.NewGuid(),
        Username = "test_user",
        NormalizedUsername = "TEST_USER",
        Password = "hashed_pw",
        FirstName = "Test",
        LastName = "User",
        Gender = Gender.Male,
        IsVerified = true
    };
    
    // TaskDTO-шаблон
    public static TaskDTO TaskDto => new TaskDTO
    {
        Title = "Title",
        Description = "Description",
        Deadline = null,
        Status = StatusTask.New,
        Priority = PriorityTask.High
    };
    public static TaskDTO UpdateDTO => new TaskDTO
    {
        Title = "New Title",
        Description = "New Descript",
        Deadline = null,
        Status = StatusTask.Done,
        Priority = PriorityTask.Medium
    };
    // DailyTask-шаблон
    public static DailyTask TestTask => new DailyTask
    {
        Id = 1,
        Uuid = Guid.NewGuid(),
        UserId = TestUser.Id,
        UserUuid = TestUser.Uuid,
        User = TestUser,
        Title = TaskDto.Title,
        Description = TaskDto.Description,
        Deadline = TaskDto.Deadline,
        Priority = TaskDto.Priority,
        Status = TaskDto.Status
    };


    // Success tests with output
    [Fact]
    public async Task CreateAndSaveTask_Succed()
    {
        // Arrange
        var userUid = TestUser.Uuid;
        var user = TestUser;
        var dto = TaskDto;
        DailyTask? createdTask = null;

        _userRepository
            .Setup(r => r.GetByUuidAsync<User>(userUid))
            .ReturnsAsync(user);

        _taskRepository
            .Setup(r => r.Add(It.IsAny<DailyTask>()))
            .Callback<DailyTask>(task => createdTask = task) 
            .Verifiable();

        // Act
        await _taskService.CreateAndSaveTask(dto, userUid);

        // Verify
        _userRepository.Verify(r => r.GetByUuidAsync<User>(userUid), Times.Once);

        _taskRepository.Verify(r => r.Add(It.Is<DailyTask>(t =>
            t.Title == dto.Title &&
            t.Description == dto.Description &&
            t.UserUuid == userUid &&
            t.UserId == user.Id
        )), Times.Once);

        _taskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);

        // Raw output
        _output.WriteLine($"Created task:");
        _output.WriteLine($"- Uuid: {createdTask?.Uuid}");
        _output.WriteLine($"- Title: {createdTask?.Title}");
        _output.WriteLine($"- Description: {createdTask?.Description}");
        _output.WriteLine($"- UserId: {createdTask?.UserId}");
        _output.WriteLine($"- UserUuid: {createdTask?.UserUuid}");
        _output.WriteLine($"- Priority: {createdTask?.Priority}");
        _output.WriteLine($"- Status: {createdTask?.Status}");
        _output.WriteLine($"- Deadline: {createdTask?.Deadline}");

        // Asserts
        Assert.NotNull(createdTask);
        Assert.Equal(dto.Title, createdTask.Title);
        Assert.Equal(dto.Description, createdTask.Description);
        Assert.Equal(userUid, createdTask.UserUuid);
        Assert.Equal(user.Id, createdTask.UserId);
        Assert.NotEqual(Guid.Empty, createdTask.Uuid);
    }
    [Fact]
    public async Task GetTask_ShouldReturnTask_WhenTaskExists()
    {
        // Arrange
        var userUid = TestUser.Uuid;
        var task = TestTask;

        _taskRepository
            .Setup(r => r.GetTaskByUserUidAsync(userUid, task.Uuid))
            .ReturnsAsync(task);

        // Act
        var result = await _taskService.GetTask(userUid, task.Uuid);

        // Assert
        _output.WriteLine("=== RETRIEVED TASK ===");
        _output.WriteLine($"- Uuid: {task.Uuid}");
        _output.WriteLine($"- Title: {task.Title}");
        _output.WriteLine($"- Description: {task.Description}");
        _output.WriteLine($"- UserId: {task.UserId}");
        _output.WriteLine($"- UserUuid: {task.UserUuid}");
        _output.WriteLine($"- Priority: {task.Priority}");
        _output.WriteLine($"- Status: {task.Status}");
        _output.WriteLine($"- Deadline: {task.Deadline}");

        Assert.NotNull(result);
        Assert.Equal(task.Uuid, result.Uuid);
        Assert.Equal(task.Title, result.Title);

        _taskRepository.Verify(r => r.GetTaskByUserUidAsync(userUid, task.Uuid), Times.Once);
    }

    [Fact]
    public async Task UpdateTask_ShouldReturnUpdatedTask_WhenTaskExists()
    {
        // Arrange
        var userUid = TestUser.Uuid;
        var task = TestTask;
        
        var updateDto = new TaskDTO
        {
            Title = "New Title",
            Description = "New Description",
            Deadline = null,
            Status = StatusTask.Done,
            Priority = PriorityTask.Medium
        };

        _taskRepository
            .Setup(r => r.GetTaskByUserUidAsync(userUid, task.Uuid))
            .ReturnsAsync(task);

        _taskRepository
            .Setup(r => r.Update(It.IsAny<DailyTask>()))
            .Verifiable();

        _output.WriteLine("=== Before update TASK ===");
        _output.WriteLine($"- Uuid: {task.Uuid}");
        _output.WriteLine($"- Title: {task.Title}");
        _output.WriteLine($"- Description: {task.Description}");
        _output.WriteLine($"- Priority: {task.Priority}");
        _output.WriteLine($"- Status: {task.Status}");
        _output.WriteLine($"- Deadline: {task.Deadline}");    

        // Act
        var result = await _taskService.UpdateTask(updateDto, userUid, task.Uuid);

        // Assert
        _output.WriteLine(" =================== ");
        _output.WriteLine("=== UPDATED TASK ===");
        _output.WriteLine($"- Uuid: {result.Uuid}");
        _output.WriteLine($"- Title: {result.Title}");
        _output.WriteLine($"- Description: {result.Description}");
        _output.WriteLine($"- Priority: {result.Priority}");
        _output.WriteLine($"- Status: {result.Status}");
        _output.WriteLine($"- Deadline: {result.Deadline}");

        Assert.NotNull(result);
        Assert.Equal(task.Uuid, result.Uuid);
        Assert.Equal(updateDto.Title, result.Title); 
        Assert.Equal(updateDto.Description, result.Description);
        Assert.Equal(updateDto.Priority, result.Priority);
        Assert.Equal(updateDto.Status, result.Status);

        _taskRepository.Verify(r => r.GetTaskByUserUidAsync(userUid, task.Uuid), Times.Once);
        _taskRepository.Verify(r => r.Update(It.IsAny<DailyTask>()), Times.Once);
        _taskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteTask_ShouldReturnUpdatedTask_WhenTaskExists()
    {
        // Arrange
        var userUid = TestUser.Uuid;
        var task = TestTask;

        _taskRepository
            .Setup(r => r.GetTaskByUserUidAsync(userUid, task.Uuid))
            .ReturnsAsync(task);

        // Act
        await _taskService.DeleteTask(userUid, task.Uuid);

        _taskRepository.Verify(r => r.GetTaskByUserUidAsync(userUid, task.Uuid), Times.Once);
    }

}