

using BusinessLogicLayerCore.Services.Interfaces;

public class DefaultAnswerStrategy : IAiAgentBehaviour
{
    private readonly IAgentService _agentService;

    public DefaultAnswerStrategy(IAgentService _agentService){
        this._agentService = _agentService;
    }

    // Данный метод будет вызван, в случае если условия остальных стратегии не были удволетворены  (как крайний)
    public bool CanHandle(string prompt) => true;

    // Основной метод
    public async Task<string> HandleAsync(string prompt, Guid uuid){
        return await Task.FromResult($"AI says: {prompt}");
    }
}
