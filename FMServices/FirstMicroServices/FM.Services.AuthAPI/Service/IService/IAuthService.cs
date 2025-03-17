using FM.Services.AuthAPI.Models.DTO;

namespace FM.Services.AuthAPI.Service.IService
{
    public interface IAuthService
    {
        Task<string> RegisterUser(RegistrationRequestDTO requestDTO);
        Task<LoginResposeDTO> LoginUser(LoginRequestDTO requestDTO);
        Task<bool> AssignRole(string email, string roleName);
    }
}
