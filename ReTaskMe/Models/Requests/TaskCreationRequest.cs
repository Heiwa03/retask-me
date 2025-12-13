
namespace ReTaskMe.Models.Requests;

public class TaskCreationRequest
{
    public string? UserDescription { get; set; }
    public string? ProjectContext { get; set; }
    public Dictionary<string, string> AdditionalParams { get; set; } = new();
}