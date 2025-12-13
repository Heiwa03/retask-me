// DataAccessLayerCore/Repositories/BoardRepository.cs
using DataAccessLayerCore.Entities;
using DataAccessLayerCore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayerCore.Repositories;

public class BoardRepository : BaseRepository, IBoardRepository 
{
    private readonly DatabaseContext _context;

    public BoardRepository(DatabaseContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<Board>> GetBoardsByUserUuidAsync(Guid userUuid)
    {
        return await _context.Boards
            .Include(b => b.DailyTasks)       
            .Where(b => b.UserUuid == userUuid)
            .ToListAsync();
    }

    public async Task<Board?> GetBoardByUserUuidAsync(Guid userUuid, Guid boardUuid)
    {
        return await _context.Boards
            .Include(b => b.DailyTasks)       
            .FirstOrDefaultAsync(b => b.UserUuid == userUuid && b.Uuid == boardUuid);
    }


}
