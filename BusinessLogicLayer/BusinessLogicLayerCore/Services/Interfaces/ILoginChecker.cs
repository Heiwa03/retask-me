using BusinessLogicLayerCore.DTOs;

namespace BusinessLogicLayerCore.Services.Interfaces
{
    public interface ILoginChecker
    {
        Task<bool> CheckCredentials(LoginDto dto);
    }
}
