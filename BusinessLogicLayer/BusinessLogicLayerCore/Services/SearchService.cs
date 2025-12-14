using BusinessLogicLayerCore.Services.Interfaces;
using DataAccessLayerCore.Entities;
using DataAccessLayerCore.Repositories.Interfaces;

namespace BusinessLogicLayerCore.Services;


public class SearchService : ISearchService{
    public ISearchBehaviour _strategy;
    public ITaskRepository _taskRepo;
    public IUserRepository _userRepo;

    public SearchService(ISearchBehaviour _strategy, ITaskRepository _taskRepo, IUserRepository _userRepo){
        this._strategy = _strategy;
        this._taskRepo = _taskRepo;
        this._userRepo = _userRepo;
    }

    public void SetStrategy(ISearchBehaviour _strategy){
        this._strategy = _strategy;
    }

    public async Task<IEnumerable<DailyTask>> SearchTasks(Guid userUuid, string query){
        var user = await _userRepo.GetByUuidAsync<User>(userUuid);
        if (user == null)
        {
            throw new KeyNotFoundException($"User {userUuid} not found");
        }

        var task = await _taskRepo.GetTasksByUserUidAsync(userUuid);

        return _strategy.Search(task, query);
    }
}