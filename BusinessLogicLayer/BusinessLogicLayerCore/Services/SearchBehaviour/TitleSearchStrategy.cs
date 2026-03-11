using BusinessLogicLayerCore.Services.Interfaces;
using DataAccessLayerCore.Entities;

namespace BusinessLogicLayerCore.Services.SearchBehaviour;


public class TitleSearchStrategy : ISearchBehaviour{
    public IEnumerable<DailyTask> Search(IEnumerable<DailyTask> tasks, string query){
        return tasks.Where(t => 
            t.Title.Contains(query, StringComparison.OrdinalIgnoreCase));
    }
}


