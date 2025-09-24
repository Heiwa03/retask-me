using BusinessLogicLayerCore.DTOs;

namespace BusinessLogicLayerCore.Services.Interfaces{
    public interface IRegisterService{
        // 1. Main method for register
        Task RegisterUser(RegisterDTO dto);
    }
}