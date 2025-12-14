

using BusinessLogicLayerCore.Services.Interfaces;

public class DefaultAnswerStrategy : IAiAgentBehaviour
{
    private readonly IAgentService _agentService;

    public DefaultAnswerStrategy(IAgentService _agentService){
        this._agentService = _agentService;
    }

    // Данный метод будет вызван если условиe остальных стратегии не были удволетворены (как крайний)
    public bool CanHandle(string prompt) => true;

    // Основной метод. Guid нужен чтобы ИИ мог обращаться по имени, но это на будущее уже
    public async Task<string> HandleAsync(string prompt, Guid uuid){
        return await _agentService.GenerateAnswer(prompt);
    }
}
