using System.ComponentModel.DataAnnotations;

namespace ReTaskMe.Models.Response{
    public class UserProfile{
        
        public string Username { get; set; } = null!; 
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int Gender {get; set; }
    }
}