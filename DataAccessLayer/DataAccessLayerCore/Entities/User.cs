using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayerCore.Entities;

[Index(nameof(Username), IsUnique = true)]
public class User : BaseId
{
    [StringLength(255)] public required string Username { get; set; }
    [StringLength(255)] public required string NormalizedUsername { get; set; }
    [StringLength(255)] public required string Password { get; set; }

    // New property to track email verification
    public bool IsVerified { get; set; } = false;
}
