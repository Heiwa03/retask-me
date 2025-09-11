

using System.ComponentModel.DataAnnotations;
using BusinessLogicLayer.Enums;
using Microsoft.EntityFrameworkCore.Query;

namespace BusinessLogicLayer.DTOs{
    public class RegisterDTO{

        [Required(ErrorMessage = "Username is required")]
        [MinLength(2, ErrorMessage = "Username is short")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters length")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Rep password is required")]
        public string RepeatPassword { get; set; } = null!;

        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Mail {get; set; }

        //public UserRole Role {get; set; }

        // TODO in future: Phone, Gender (girls = 0, boys = 1), Role
    }
}
