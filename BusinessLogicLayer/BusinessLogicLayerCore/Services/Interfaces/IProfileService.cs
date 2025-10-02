using BusinessLogicLayerCore.DTOs;

namespace BusinessLogicLayerCore.Services.Interfaces;

    public interface IProfileService{
        Task RegisterUserProfile(PostRegisterDTO dto, Guid userUuid);
        Task<PostRegisterDTO> GetProfile(Guid uuid);
        Task UpdateProfile(PostRegisterDTO dto, Guid userUid);
    }