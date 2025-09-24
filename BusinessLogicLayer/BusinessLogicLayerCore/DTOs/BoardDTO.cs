// BusinessLogicLayerCore/DTOs/BoardDTO.cs
namespace BusinessLogicLayerCore.DTOs
{
    public class BoardDTO
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public ICollection<TaskDTO>? Tasks { get; set; }
    }
}