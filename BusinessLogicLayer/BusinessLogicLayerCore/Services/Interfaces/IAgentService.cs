using System.Xml.Serialization;

using BusinessLogicLayerCore.DTOs;

namespace BusinessLogicLayerCore.Services.Interfaces;

public interface IAgentService
{
    Task<string> GenerateTaskAsync(string userInput, string? context = null);

}