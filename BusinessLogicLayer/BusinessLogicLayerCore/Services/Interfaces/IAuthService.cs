using System.Threading.Tasks;

namespace BusinessLogicLayerCore.Services.Interfaces
{
    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }

    public interface IAuthService
    {
        Task<AuthResponse?> LoginAsync(string email, string password);
        Task<AuthResponse?> RefreshAsync(string refreshToken);
    }
}
