using System.Xml.Serialization;

using BusinessLogicLayerCore.DTOs;

namespace BusinessLogicLayerCore.Services.Interfaces;

public interface IAgentService{
    Task<TaskDTO> GenerateTask(string userPromnt);
    //Task AddGeneratedTask(Guid uuid, string prompt);
}