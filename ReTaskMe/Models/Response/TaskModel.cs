// В Web/Models/Responses
namespace ReTaskMe.Models.Response{
    public class TaskResponseModel {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? Deadline { get; set; }
        public string Status { get; set; } = null!;
        public string Priority { get; set; } = null!;
    }
}
