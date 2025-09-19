// Microsoft
using Microsoft.EntityFrameworkCore;


// DAL
using DataAccessLayerCore.Entities;
using DataAccessLayerCore.Repositories.Interfaces;


namespace DataAccessLayerCore.Repositories
{
    public class TaskRepository : BaseRepository, ITaskRepository
    {
        private readonly DatabaseContext _context;

        public TaskRepository(DatabaseContext context) : base(context){
            _context = context;
        }

        // Специфичные методы для TaskModel

        /// <summary>
        /// Получить все задачи пользователя
        /// </summary>
        public async Task<List<DailyTask>> GetTasksByUserIdAsync(long userId)
        {
            return await _context.Tasks
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        // найти юзера и таск
        public async Task<DailyTask?> GetTaskByUserAndIdAsync(long userId, long taskId)
        {
            return await _context.Tasks
                .FirstOrDefaultAsync(t => t.UserId == userId && t.Id == taskId);
        }

        /// <summary>
        /// Получить задачи по статусу
        /// </summary>
        // public async Task<List<DailyTask>> GetTasksByStatusAsync(long userId, StatusTask status)
        // {
        //     return await _context.Tasks
        //         .Where(t => t.UserId == userId && t.Status == status)
        //         .ToListAsync();
        // }

        /// <summary>
        /// Получить задачи по бордеру
        /// </summary>
        // public async Task<List<TaskModel>> GetTasksByBoardIdAsync(int boardId)
        // {
        //     return await _context.Tasks
        //         .Where(t => t.BoardId == boardId)
        //         .ToListAsync();
        // }
    }
}
