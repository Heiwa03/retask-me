

using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.testsBagrin.Entity;


namespace BusinessLogicLayer.Services.Interfaces{
    public interface IRegisterService{
        // 1. Main method for register
        Task RegisterUser(RegisterDTO dto);
    }
}