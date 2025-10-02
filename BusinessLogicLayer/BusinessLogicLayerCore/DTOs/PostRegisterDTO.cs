using System.ComponentModel.DataAnnotations;
using DataAccessLayerCore.Enum;

namespace BusinessLogicLayerCore.DTOs{
    public class PostRegisterDTO{

        [Required(ErrorMessage = "Firstname is required")]
        [MinLength(1, ErrorMessage = "Firstname is short")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Lastname is required")]
        [MinLength(1, ErrorMessage = "Lastname is short")]
        public string LastName { get; set; } = null!;

        public Gender Gender {get; set; } = Gender.Undefiend;
    }
}