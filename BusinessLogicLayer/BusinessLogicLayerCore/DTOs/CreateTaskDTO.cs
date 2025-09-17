// System
using System.ComponentModel.DataAnnotations;

// DAL
using DataAccessLayerCore.Enum;

namespace BusinessLogicLayerCore.DTOs{
    public class CreateTaskDTO{
        [Required(ErrorMessage = "Title required")]
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public StatusTask Status { get; set; }
        public PriorityTask Priority { get; set; }
    }
}