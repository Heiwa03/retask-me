namespace DataAccessLayerCore.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAccessLayerCore.Enum;
using Microsoft.EntityFrameworkCore;

[Index(nameof(Id), IsUnique = true)]
public class Board : BaseId 
{
    public long UserId { get; set; }
    public Guid UserUuid { get; set; }
    [ForeignKey(nameof(UserId))] public required User User { get; set; }
    
    [StringLength(50)] public required string Title { get; set; }
    [StringLength(255)] public string? Description { get; set; }

    public required virtual ICollection<DailyTask> DailyTasks { get; set; }
}