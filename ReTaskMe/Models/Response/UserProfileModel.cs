using System.ComponentModel.DataAnnotations;
using DataAccessLayerCore.Enum;

namespace ReTaskMe.Models.Response{
    public class UserProfileModel{        
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public Gender Gender {get; set; }
    }
}