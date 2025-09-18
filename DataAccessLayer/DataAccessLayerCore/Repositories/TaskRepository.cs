// Microsoft
using Microsoft.EntityFrameworkCore;


// DAL
using DataAccessLayerCore.Entities;
using DataAccessLayerCore.Repositories.Interfaces;
using DataAccessLayerCore.Enum;



namespace DataAccessLayerCore.Repositories
{
    public class TaskRepository : BaseRepository, ITaskRepository
    {
        private readonly DatabaseContext _context;

        public TaskRepository(DatabaseContext context) : base(context)
        {
            _context = context;
        }

        // Специфичные методы для TaskModel

        /// <summary>
        /// Получить все задачи пользователя
        /// </summary>
        public async Task<List<TaskModel>> GetTasksByUserIdAsync(int userId)
        {
            return await _context.Tasks
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        /// <summary>
        /// Получить задачи по статусу
        /// </summary>
        public async Task<List<TaskModel>> GetTasksByStatusAsync(int userId, StatusTask status)
        {
            return await _context.Tasks
                .Where(t => t.UserId == userId && t.Status == status)
                .ToListAsync();
        }

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
