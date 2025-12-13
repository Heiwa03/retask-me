// BusinessLogicLayerCore/Services/BoardService.cs
using BusinessLogicLayerCore.Services.Interfaces;
using BusinessLogicLayerCore.DTOs;
using DataAccessLayerCore.Entities;
using DataAccessLayerCore.Repositories.Interfaces;
using DataAccessLayerCore.Repositories;


namespace BusinessLogicLayerCore.Services;

public class BoardService : IBoardService
{
    private readonly IBoardRepository _boardRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITaskRepository _taskRepository;

    public BoardService(
        IBoardRepository boardRepository,
        IUserRepository userRepository,
        ITaskRepository taskRepository)
    {
        _boardRepository = boardRepository;
        _userRepository = userRepository;
        _taskRepository = taskRepository; 
    }

    public async Task CreateBoard(BoardDTO boardDto, Guid userUuid)
    {
        var user = await _userRepository.GetByUuidAsync<User>(userUuid) 
            ?? throw new KeyNotFoundException($"User {userUuid} not found");

        var board = new Board
        {
            Uuid = Guid.NewGuid(),
            Title = boardDto.Title,
            Description = boardDto.Description,
            UserId = user.Id,
            UserUuid = user.Uuid,
            User = user,
            DailyTasks = new List<DailyTask>()
        };

        _boardRepository.Add(board);
        await _boardRepository.SaveChangesAsync();
    }

    public async Task AddTaskToBoard(Guid userUuid, Guid boardUuid, Guid taskUuid)
    {
        var board = await _boardRepository.GetBoardByUserUuidAsync(userUuid, boardUuid)
            ?? throw new KeyNotFoundException($"Board {boardUuid} not found");

        var task = await _taskRepository.GetTaskByUserUidAsync(userUuid, taskUuid)
            ?? throw new KeyNotFoundException($"Task {taskUuid} not found");
        
        task.BoardId = board.Id;
        task.BoardUuid = board.Uuid;
        task.Board = board;

        var taskDto = new TaskDTO
        {
            Title = task.Title,
            Description = task.Description,
            Deadline = task.Deadline,
            Priority = task.Priority,
            Status = task.Status
        };

        _boardRepository.Update(board);
    }

    public async Task RemoveTaskFromBoard(Guid userUuid, Guid boardUuid, Guid taskUuid)
    {
        var board = await _boardRepository.GetBoardByUserUuidAsync(userUuid, boardUuid)
            ?? throw new KeyNotFoundException($"Board {boardUuid} not found");

        var task = await _taskRepository.GetTaskByUserUidAsync(userUuid, taskUuid)
            ?? throw new KeyNotFoundException($"Task {taskUuid} not found");
        
        if (task.BoardUuid != boardUuid)
            throw new InvalidOperationException("Task is not in this board");

        task.BoardId = null;
        task.BoardUuid = null;
        task.Board = null;

        var taskDto = new TaskDTO
        {
            Title = task.Title,
            Description = task.Description,
            Deadline = task.Deadline,
            Priority = task.Priority,
            Status = task.Status
        };

        _boardRepository.Update(board);      
    }

    public async Task<List<BoardDTO>> GetUserBoards(Guid userUuid)
    {
        var boards = await _boardRepository.GetBoardsByUserUuidAsync(userUuid);
        return boards.Select(ToBoardDTO).ToList();
    }

    public async Task<BoardDTO> GetBoardWithTasks(Guid userUuid, Guid boardUuid)
    {
        var board = await _boardRepository.GetBoardByUserUuidAsync(userUuid, boardUuid)
            ?? throw new KeyNotFoundException($"Board {boardUuid} not found");

        return ToBoardDTO(board);
    }


    public async Task<List<DailyTask>> GetTasksFromBoard(Guid userUuid, Guid boardUuid)
    {
        var board = await _boardRepository.GetBoardByUserUuidAsync(userUuid, boardUuid)
            ?? throw new KeyNotFoundException($"Board {boardUuid} not found");

        return board.DailyTasks.ToList();
    }

    private BoardDTO ToBoardDTO(Board board)
    {
        return new BoardDTO
        {
            Title = board.Title,
            Description = board.Description,
            Tasks = board.DailyTasks?.Select(t => new TaskDTO
            {
                Title = t.Title,
                Description = t.Description,
                Deadline = t.Deadline,
                Priority = t.Priority,
                Status = t.Status
            }).ToList()
        };
    }
}