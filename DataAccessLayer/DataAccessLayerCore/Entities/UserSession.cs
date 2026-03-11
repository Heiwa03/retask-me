using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayerCore.Entities
{
    public class UserSession : BaseId
    {
        public long UserId { get; set; }

        [StringLength(250)] public string? RefreshToken { get; set; }

        [StringLength(36)] public string? JwtId { get; set; }

        public DateTime RefreshTokenExpiration { get; set; }

        // This marks if the refresh token has already been used once. 
        public bool Redeemed { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
    }
}
