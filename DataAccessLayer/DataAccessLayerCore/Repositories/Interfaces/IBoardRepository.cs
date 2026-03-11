// DataAccessLayerCore/Repositories/Interfaces/IBoardRepository.cs
using DataAccessLayerCore.Entities;


namespace DataAccessLayerCore.Repositories.Interfaces;

public interface IBoardRepository : IBaseRepository
{
    Task<List<Board>> GetBoardsByUserUuidAsync(Guid userUuid);
    Task<Board?> GetBoardByUserUuidAsync(Guid userUuid, Guid boardUuid);

}