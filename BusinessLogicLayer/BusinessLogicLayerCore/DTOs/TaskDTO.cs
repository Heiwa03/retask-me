// DAL
using DataAccessLayerCore.Enum;

namespace BusinessLogicLayerCore.DTOs{

    public class TaskDTO {
        public int Id { get; set; }             
        public int UserId { get; set; }          
        public string Title { get; set; } = null!;        
        public string? Description { get; set; } 
        public DateTime CreatedAt { get; set; }  
        public DateTime? UpdatedAt { get; set; } 
        public DateTime? Deadline { get; set; }   
        public StatusTask Status { get; set; }  
        public PriorityTask Priority { get; set; } 
    }
}