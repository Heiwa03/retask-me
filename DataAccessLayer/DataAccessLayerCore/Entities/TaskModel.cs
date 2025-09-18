using DataAccessLayerCore.Enum;

namespace DataAccessLayerCore.Entities{

    public class TaskModel{
        public int Id { get; set; }
        public int UserId { get; set; }      
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? Deadline { get; set; }
        public PriorityTask Priority { get; set; }
        public StatusTask Status {get; set; }
        //public BoardModel Board { get; set; } = null!;
        //public int BoardId { get; set; }     
    }
}


