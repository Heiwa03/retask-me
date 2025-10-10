

using BusinessLogicLayerCore.DTOs;
using BusinessLogicLayerCore.Services.Interfaces;
using DataAccessLayerCore.Entities;

namespace BusinessLogicLayerCore.Services;

    public class UserService : IUserService {
        private readonly ITaskService _taskService;
        private readonly IProfileService _profileService;
        private readonly IBoardService _boardService;

        public UserService(ITaskService _taskService, IProfileService _profileService, IBoardService _boardService)
        {
            this._taskService = _taskService ?? throw new ArgumentNullException(nameof(_taskService));
            this._profileService = _profileService ?? throw new ArgumentNullException(nameof(_profileService));
            this._boardService = _boardService ?? throw new ArgumentNullException(nameof(_boardService));
        }

        // --------------------
        // Delegete to taskServ
        // --------------------
        public async Task CreateTask(TaskDTO dto, Guid userUid){
            await _taskService.CreateAndSaveTask(dto, userUid);
        }

        public async Task<DailyTask> GetTask(Guid userUid, Guid taskUid){
            return await _taskService.GetTask(userUid, taskUid);
        }

        public async Task<List<DailyTask>> GetAllTasks(Guid userUid){
            return await _taskService.GetAllTasks(userUid);
        }

        public async Task UpdateTask(TaskDTO dto, Guid userUid, Guid taskUid){
            await _taskService.UpdateTask(dto, userUid, taskUid);
        }

        public async Task DeleteTask(Guid userUid, Guid taskUid){
            await _taskService.DeleteTask(userUid, taskUid);
        }
        
        // --------------------
        // Delegete to profServ
        // --------------------
        public async Task<PostRegisterDTO> GetUserProfile(Guid userUid){
            return await _profileService.GetProfile(userUid);
        }

        public async Task UpdateUserProfile(PostRegisterDTO dto, Guid userUid){
            await _profileService.UpdateProfile(dto, userUid);
        }

        // ---------------------
        // Delegete to boardServ
        // ---------------------
        public async Task CreateBoard(BoardDTO boardDto, Guid userUuid){
            await _boardService.CreateBoard(boardDto, userUuid);
        }

        public async Task AddTaskToBoard(Guid userUuid, Guid boardUuid, Guid taskUuid){
            await _boardService.AddTaskToBoard(userUuid, boardUuid, taskUuid);
        }

        public async Task RemoveTaskFromBoard(Guid userUuid, Guid boardUuid, Guid taskUuid){
            await _boardService.RemoveTaskFromBoard(userUuid, boardUuid, taskUuid);
        }

        public async Task<List<BoardDTO>> GetUserBoards(Guid userUuid){
            return await _boardService.GetUserBoards(userUuid);
        }
        // public async Task<BoardDTO> GetBoardWithTasks(Guid userUuid, Guid boardUuid){
        //     return await _boardService.GetBoardWithTasks(userUuid, boardUuid);
        // }
        public async Task<List<TaskDTO>> GetTasksFromBoard(Guid userUuid, Guid boardUuid){
            return await _boardService.GetTasksFromBoard(userUuid, boardUuid);
        }
    }
    
        

