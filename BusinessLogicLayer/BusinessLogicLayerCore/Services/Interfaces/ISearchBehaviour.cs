using DataAccessLayerCore.Entities;

namespace BusinessLogicLayerCore.Services.Interfaces;


public interface ISearchBehaviour{
    IEnumerable<DailyTask> Search(IEnumerable<DailyTask> tasks, string query);
}