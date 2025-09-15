

using System.ComponentModel.DataAnnotations;
using BusinessLogicLayer.Enums;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Query;

namespace BusinessLogicLayer.DTOs{
    public class RegisterDTO{

        [Required(ErrorMessage = "Mail is required")]
        [EmailAddress(ErrorMessage = "Invalid mail format")]
        public string Mail { get; set; } = null!; 

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters length")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Rep password is required")]
        public string RepeatPassword { get; set; } = null!;

        public UserRole Role {get; set; } = UserRole.Client;
    }
}
