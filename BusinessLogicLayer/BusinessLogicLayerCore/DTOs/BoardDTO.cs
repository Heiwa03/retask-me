// BusinessLogicLayerCore/DTOs/BoardDTO.cs
namespace BusinessLogicLayerCore.DTOs
{
    public class BoardDTO
    {
        public Guid Uuid { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
    }
}