using BusinessLogicLayerCore.DTOs;
using DataAccessLayerCore.Entities;

namespace BusinessLogicLayerCore.Services.Interfaces{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginDto loginDto);

        Task<AuthResponse?> RefreshAsync(string refreshToken);
    }
}
