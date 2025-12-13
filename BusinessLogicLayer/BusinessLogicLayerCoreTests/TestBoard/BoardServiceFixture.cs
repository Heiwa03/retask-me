using Xunit;
using Moq;
using Xunit.Abstractions;
using BusinessLogicLayerCore.Services;
using BusinessLogicLayerCore.DTOs;
using DataAccessLayerCore.Entities;
using DataAccessLayerCore.Repositories.Interfaces;
using DataAccessLayerCore.Enum;

namespace BusinessLogicLayerCoreTests.TestBoard;

public class BoardServiceFixture
{
    private readonly Mock<IBoardRepository> _boardRepository = new();
    private readonly Mock<ITaskRepository> _taskRepository = new();
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly BoardService _boardService;
    private readonly ITestOutputHelper _output;

    public BoardServiceFixture(ITestOutputHelper output)
    {
        _boardService = new BoardService(_boardRepository.Object, _userRepository.Object, _taskRepository.Object);
        _output = output;
    }

    // Тестовый пользователь
    public User TestUser => new User
    {
        Id = 123,
        Uuid = Guid.Parse("df38fb40-f862-469c-b081-6255edbf7114"),
        Username = "test_user",
        NormalizedUsername = "TEST_USER",
        Password = "3cm",
        FirstName = "Lastname",
        LastName = "Firstname",
        Gender = Gender.Male,
        IsVerified = true
    };

    // Тестовая доска
    public Board TestBoard => new Board
    {
        Id = 1,
        Uuid = Guid.NewGuid(),
        UserId = TestUser.Id,
        UserUuid = TestUser.Uuid,
        User = TestUser,
        Title = "Test Board",
        Description = "Test Board Description",
        DailyTasks = new List<DailyTask>()
    };

    // Тестовые задачи
    public DailyTask TestTask => new DailyTask
    {
        Id = 1,
        Uuid = Guid.NewGuid(),
        UserId = TestUser.Id,
        UserUuid = TestUser.Uuid,
        BoardId = TestBoard.Id,
        BoardUuid = TestBoard.Uuid,
        Board = TestBoard,
        User = TestUser,
        Title = "Test Task",
        Description = "Test Description",
        Priority = PriorityTask.High,
        Status = StatusTask.New
    };
    public DailyTask TestTask2 => new DailyTask
    {
        Id = 2,
        Uuid = Guid.NewGuid(),
        UserId = TestUser.Id,
        UserUuid = TestUser.Uuid,
        BoardId = TestBoard.Id,
        BoardUuid = TestBoard.Uuid,
        Board = TestBoard,
        User = TestUser,
        Title = "Test Task222",
        Description = "Test Description2222",
        Priority = PriorityTask.High,
        Status = StatusTask.New
    };

    // BoardDTO шаблон
    public BoardDTO BoardDto => new BoardDTO
    {
        Title = "Test Board",
        Description = "Test Board Description"
    };

    [Fact]
    public async Task CreateBoard_ShouldCallAddAndSave_WhenUserExists()
    {
        // Arrange
        var userUid = TestUser.Uuid;
        Board? createdBoard = null;

        _userRepository
            .Setup(r => r.GetByUuidAsync<User>(userUid))
            .ReturnsAsync(TestUser);

        _boardRepository
            .Setup(r => r.Add(It.IsAny<Board>()))
            .Callback<Board>(b => createdBoard = b);

        _boardRepository
            .Setup(r => r.SaveChangesAsync());

        // Act
        await _boardService.CreateBoard(BoardDto, userUid);

        // Assert
        Assert.NotNull(createdBoard);
        Assert.Equal(BoardDto.Title, createdBoard!.Title);
        Assert.Equal(BoardDto.Description, createdBoard.Description);
        Assert.Equal(TestUser.Id, createdBoard.UserId);
        Assert.Equal(TestUser.Uuid, createdBoard.UserUuid);

        _userRepository.Verify(r => r.GetByUuidAsync<User>(userUid), Times.Once);
        _boardRepository.Verify(r => r.Add(It.IsAny<Board>()), Times.Once);
        _boardRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AddTaskToBoard_ShouldAddTask_WhenBoardAndTaskExist()
    {
        // Arrange
        var userUuid = TestUser.Uuid;
        var board = TestBoard;
        var task = TestTask;

        _boardRepository
            .Setup(r => r.GetBoardByUserUuidAsync(userUuid, board.Uuid))
            .ReturnsAsync(board);

        _taskRepository
            .Setup(r => r.GetTaskByUserUidAsync(userUuid, task.Uuid))
            .ReturnsAsync(task);

        _boardRepository
            .Setup(r => r.Update(board))
            .Verifiable();

        // Act
        await _boardService.AddTaskToBoard(userUuid, board.Uuid, task.Uuid);

        // Assert
        Assert.Equal(board.Id, task.BoardId);
        Assert.Equal(board.Uuid, task.BoardUuid);
        Assert.Equal(board, task.Board);

        _boardRepository.Verify(r => r.Update(board), Times.Once);
    }

    [Fact]
    public async Task RemoveTaskFromBoard_ShouldRemoveTask_WhenTaskInBoard()
    {
        // Arrange
        var userUuid = TestUser.Uuid;
        var board = TestBoard;
        var task = TestTask;

        task.BoardId = board.Id;
        task.BoardUuid = board.Uuid;
        task.Board = board;

        _boardRepository
            .Setup(r => r.GetBoardByUserUuidAsync(userUuid, board.Uuid))
            .ReturnsAsync(board);

        _taskRepository
            .Setup(r => r.GetTaskByUserUidAsync(userUuid, task.Uuid))
            .ReturnsAsync(task);

        _boardRepository
            .Setup(r => r.Update(board))
            .Verifiable();

        // Act
        await _boardService.RemoveTaskFromBoard(userUuid, board.Uuid, task.Uuid);

        // Assert
        Assert.Null(task.BoardId);
        Assert.Null(task.BoardUuid);
        Assert.Null(task.Board);

        _boardRepository.Verify(r => r.Update(board), Times.Once);
    }

    [Fact]
    public async Task GetUserBoards_ShouldReturnBoardDTOList()
    {
        // Arrange
        var userUuid = TestUser.Uuid;
        var boards = new List<Board> { TestBoard };

        _boardRepository
            .Setup(r => r.GetBoardsByUserUuidAsync(userUuid))
            .ReturnsAsync(boards);

        // Act
        var result = await _boardService.GetUserBoards(userUuid);

        // Assert
        Assert.Single(result);
        Assert.Equal(TestBoard.Title, result[0].Title);
        Assert.Equal(TestBoard.Description, result[0].Description);
    }

    [Fact]
    public async Task GetBoardWithTasks_ShouldReturnBoardDTO_WhenBoardExists()
    {
        // Arrange
        var userUuid = TestUser.Uuid;
        var board = TestBoard;

        _boardRepository
            .Setup(r => r.GetBoardByUserUuidAsync(userUuid, board.Uuid))
            .ReturnsAsync(board);

        // Act
        var result = await _boardService.GetBoardWithTasks(userUuid, board.Uuid);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(board.Title, result.Title);
        Assert.Equal(board.Description, result.Description);
    }

    [Fact]
    public async Task GetTasksFromBoard_ShouldReturnTaskList_WhenBoardExists()
    {
        // Arrange
        var userUuid = TestUser.Uuid;
        var board = TestBoard;
        foreach (var t in new [] { TestTask, TestTask2 }){
            board.DailyTasks.Add(t);
        }

        _boardRepository
            .Setup(r => r.GetBoardByUserUuidAsync(userUuid, board.Uuid))
            .ReturnsAsync(board);

        // Act
        var result = await _boardService.GetTasksFromBoard(userUuid, board.Uuid);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, t => t.Title == TestTask.Title);
        Assert.Contains(result, t => t.Title == TestTask2.Title);
    }


}