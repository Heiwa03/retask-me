<<<<<<< HEAD
using BusinessLogicLayer.DTOs;

=======


using BusinessLogicLayer.DTOs;


>>>>>>> regist+helper
namespace BusinessLogicLayer.Services.Interfaces{
    public interface IRegisterService{
        // 1. Main method for register
        Task RegisterUser(RegisterDTO dto);
    }
}