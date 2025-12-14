using DataAccessLayerCore.Entities;

namespace BusinessLogicLayerCore.Services.Interfaces;

public interface ISearchService{
    Task<IEnumerable<DailyTask>> SearchTasks(Guid userUuid, string query);
    void SetStrategy(ISearchBehaviour _strategy);
}