using FM.Web.Models;
using FM.Web.Services.IService;
using static FM.Web.Utility.SD;

namespace FM.Web.Services
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService _baseService;
        public AuthService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDTO?> AssignRoleAsync(RegistrationRequestDTO requestDTO)
        {
            return await _baseService.SendAsync(
            new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = requestDTO,
                Url = AuthApiBase + "/api/auth/assignRole"
            });

        }

        public async Task<ResponseDTO?> LoginAsync(LoginRequestDTO loginRequestDTO)
        {
            return await _baseService.SendAsync(
            new RequestDTO()
            {
               ApiType = ApiType.POST,
               Data = loginRequestDTO,
               Url = AuthApiBase + "/api/auth/login"
            },
            isWithBearer:false
           );
        }

        public async Task<ResponseDTO?> RegisterAsync(RegistrationRequestDTO requestDTO)
        {
            return await _baseService.SendAsync(
            new RequestDTO()
            {
               ApiType = ApiType.POST,
               Data = requestDTO,
               Url = AuthApiBase + "/api/auth/register"
            },isWithBearer: false
           );
        }
    }
}
