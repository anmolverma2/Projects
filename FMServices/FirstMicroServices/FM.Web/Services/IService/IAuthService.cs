using FM.Web.Models;

namespace FM.Web.Services.IService
{
    public interface IAuthService
    {
        Task<ResponseDTO?> LoginAsync(LoginRequestDTO loginRequestDTO);
        Task<ResponseDTO?> RegisterAsync(RegistrationRequestDTO requestDTO);
        Task<ResponseDTO?> AssignRoleAsync(RegistrationRequestDTO requestDTO);
    }
}
