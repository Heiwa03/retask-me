using System.ComponentModel.DataAnnotations;

namespace BusinessLogicLayer.DTOs{
    public class PostRegisterDTO{
        
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = null!; 

        [Required(ErrorMessage = "Firstname is required")]
        [MinLength(1, ErrorMessage = "Firstname is short")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Lastname is required")]
        [MinLength(1, ErrorMessage = "Lastname is short")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "Gender is required")]
        public int Gender {get; set; }
    }
}