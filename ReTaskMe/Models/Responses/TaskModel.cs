// В Web/Models/Responses
using DataAccessLayerCore.Enum;

namespace ReTaskMe.Models.Responses{
    public class TaskModel {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? Deadline { get; set; }
        public StatusTask Status { get; set; } 
        public PriorityTask Priority { get; set; }
    }
}