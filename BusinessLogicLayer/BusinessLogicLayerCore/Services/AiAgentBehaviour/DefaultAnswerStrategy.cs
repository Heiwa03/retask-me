

using BusinessLogicLayerCore.Services.Interfaces;

public class DefaultAnswerStrategy : IAiAgentBehaviour
{
    private readonly IAgentService _agentService;

    public DefaultAnswerStrategy(IAgentService _agentService){
        this._agentService = _agentService;
    }

    public bool CanHandle(string prompt) => true;

    public async Task<string> HandleAsync(string prompt, Guid uuid){
        return await Task.FromResult($"AI says: {prompt}");
    }
}
