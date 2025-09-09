using System.ComponentModel.DataAnnotations;

namespace BusinessLogicLayer.DTOs{
    public class RegisterDTO{

        [Required(ErrorMessage = "Username is required")]
        [MinLength(3, ErrorMessage = "Username is short")]
        [MaxLength(100, ErrorMessage = "username is long asf")]
        public string Username { get; set; } = null!;


        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password is shorter than...")]
        [MaxLength(20, ErrorMessage = "Password is huge bro")]
        public string Password { get; set; } = null!;


        [Required(ErrorMessage = "Rep password is required")]
        [MinLength(8, ErrorMessage = "Rep password is shorter than...")]
        [MaxLength(20, ErrorMessage = "Rep password is huge bro")]
        public string RepeatPassword { get; set; } = null!;
    }
}
