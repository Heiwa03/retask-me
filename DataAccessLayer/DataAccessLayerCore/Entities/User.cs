using DataAccessLayerCore.Enum;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayerCore.Entities;

[Index(nameof(Username), IsUnique = true)]
public class User : BaseId
{
    [StringLength(255)] public required string Username { get; set; }
    [StringLength(255)] public required string NormalizedUsername { get; set; }
    [StringLength(255)] public required string Password { get; set; }
    
    // From mentor
    [StringLength(50)] public string? FirstName { get; set; }
    [StringLength(50)] public string? LastName { get; set; }
    public Gender Gender {get; set; }
    public bool IsVerified { get; set; } = false;
}