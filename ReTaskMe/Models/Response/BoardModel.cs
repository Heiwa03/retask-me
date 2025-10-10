using BusinessLogicLayerCore.DTOs;

namespace ReTaskMe.Models.Response;

public class BoardModel{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public ICollection<TaskDTO>? Tasks { get; set; }
}