using BusinessLogicLayerCore.Services.Interfaces;
using DataAccessLayerCore.Entities;

namespace BusinessLogicLayerCore.Services;


public class SearchService{
    public ISearchService _strategy;


    public SearchService(ISearchService _strategy){
        this._strategy = _strategy;
    }

    public void SetStrategy(ISearchService _strategy){
        this._strategy = _strategy;
    }

    public IEnumerable<DailyTask> SearchTasks(IEnumerable<DailyTask> tasks, string nameTask){
        return _strategy.Search(tasks, nameTask);
    }
}