using BusinessLogicLayerCore.Services.Interfaces;
using DataAccessLayerCore.Entities;

namespace BusinessLogicLayerCore.Services.SearchBehaviour;


public class TitleSearchStrategy : ISearchService{
    public IEnumerable<DailyTask> Search(IEnumerable<DailyTask> tasks, string taskName){
        return tasks.Where(t => t.Title.Contains(taskName, StringComparison.OrdinalIgnoreCase));
    }
}