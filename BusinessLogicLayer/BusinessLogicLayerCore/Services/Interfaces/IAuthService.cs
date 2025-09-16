using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.Interfaces
{
    public class AuthResponse
    {
        public string Token { get; set; }

        public string RefreshToken { get; set; }
    }

    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(string username, string password);
        Task<AuthResponse> RefreshAsync(string refreshToken);
    }
}