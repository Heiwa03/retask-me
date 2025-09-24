namespace DataAccessLayerCore.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAccessLayerCore.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

[Index(nameof(Id), IsUnique = true)]
public class DailyTask : BaseId 
{
    public long UserId {get; set; }
    public Guid UserUuid { get; set; }
    public virtual required User User { get; set; }

    public long? BoardId { get; set; }
    public Guid? BoardUuid { get; set; }
    [ForeignKey(nameof(BoardId))] public Board? Board { get; set; }

    [StringLength(50)] public required string Title { get; set; }
    [StringLength(255)] public string? Description { get; set; }
    public DateTime? Deadline { get; set; } = DateTime.Now.AddDays(1);
    public PriorityTask Priority { get; set; } = PriorityTask.Medium;
    public StatusTask Status {get; set; } = StatusTask.New;

}