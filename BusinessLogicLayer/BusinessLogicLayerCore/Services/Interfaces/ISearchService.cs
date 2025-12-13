using DataAccessLayerCore.Entities;

namespace BusinessLogicLayerCore.Services.Interfaces;


public interface ISearchService{
    IEnumerable<DailyTask> Search(IEnumerable<DailyTask> tasks, string taskName);
}