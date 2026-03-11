using BusinessLogicLayerCore.DTOs;
using BusinessLogicLayerCore.Services.Interfaces;

namespace BusinessLogicLayerCore.Services.AiAgentBehaviour;

public class CreataTaskStrategy : IAiAgentBehaviour{
    private readonly ITaskService _taskService;
    private readonly IAgentService _agentService;

    public CreataTaskStrategy(ITaskService _taskService, IAgentService _agentService){
        this._taskService = _taskService;
        this._agentService = _agentService;
    }

    public bool CanHandle(string prompt){
        return prompt.StartsWith("create task", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<string> HandleAsync(string prompt, Guid uuid){
        try{
            TaskDTO taskDto = await _agentService.GenerateTask(prompt);
            await _taskService.CreateAndSaveTask(taskDto, uuid);
            return $"[*] Success: Task created";
        }
        catch (Exception ex){
            throw new Exception($"[*] Error: Failed to add generated task: {ex.Message}", ex);
        }
    }
}