using DataAccessLayerCore.Enum;

namespace DataAccessLayerCore.Entities{

    public class DailyTask : BaseId{
        public Guid TaskUid { get; set; }
        public Guid UserUid { get; set; }      
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? Deadline { get; set; }
        public PriorityTask Priority { get; set; }
        public StatusTask Status {get; set; } = StatusTask.InProgress;
    }
}


