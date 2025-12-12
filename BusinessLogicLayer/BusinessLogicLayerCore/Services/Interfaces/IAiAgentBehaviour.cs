namespace BusinessLogicLayerCore.Services.Interfaces;


// this interfaces helps with realization Strategy Pattern
public interface IAiAgentBehaviour{
    bool CanHandle(string prompt);
    Task<string> HandleAsync(string prompt, Guid uuid);
}