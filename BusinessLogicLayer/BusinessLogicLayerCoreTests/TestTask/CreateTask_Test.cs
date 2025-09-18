using Xunit;
using BusinessLogicLayerCore.Services;
using BusinessLogicLayerCore.DTOs;
using DataAccessLayerCore.Enum;

namespace BusinessLogicLayerCoreTests.TestTask{
    public class CreateTask_Test{
    [Fact]
    public async Task CreateTask_ShouldReturnTaskModel_WhenValidDTO()
    {
        // Arrange
        var service = new TaskService();
        var dto = new TaskDTO
        {
            Title = "Test Task",
            Description = "Some description",
            Deadline = DateTime.Now.AddDays(7),
            Priority = PriorityTask.High
        };
        int userId = 42;

        // Act
        var result = await service.CreateTask(dto, userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Title, result.Title);
        Assert.Equal(dto.Description, result.Description);
        Assert.Equal(dto.Deadline, result.Deadline);
        Assert.Equal(dto.Priority, result.Priority);
        Assert.Equal(userId, result.UserId);
    }
}
}

