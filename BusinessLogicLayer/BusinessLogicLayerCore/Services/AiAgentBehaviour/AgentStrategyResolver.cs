
using BusinessLogicLayerCore.Services.Interfaces;

namespace BusinessLogicLayerCore.Services.AiAgentBehaviour;


public class AgentStrategyResolver : IAgentStrategyResolver
{
    private readonly IEnumerable<IAiAgentBehaviour> _strategies;

    public AgentStrategyResolver(IEnumerable<IAiAgentBehaviour> strategies)
    {
        _strategies = strategies;
    }

    public IAiAgentBehaviour Resolve(string prompt)
    {
        return _strategies.First(s => s.CanHandle(prompt));
    }
}
