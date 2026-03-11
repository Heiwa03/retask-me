


namespace BusinessLogicLayerCore.Services.Interfaces;

public interface IAgentStrategyResolver{
    public IAiAgentBehaviour Resolve(string prompt);
}