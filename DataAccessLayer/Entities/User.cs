using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entities;

[Index(nameof(Username), IsUnique = true)]
public class User : BaseId
{
    [StringLength(255)] public required string Username { get; set; }
    [StringLength(255)] public required string Password { get; set; }
    //public required DateTime RegistrationDate { get; set; }
}