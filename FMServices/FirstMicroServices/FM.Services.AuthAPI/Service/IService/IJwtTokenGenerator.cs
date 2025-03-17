using FM.Services.AuthAPI.Models;

namespace FM.Services.AuthAPI.Service.IService
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(ApplicationUser applicationUser,IEnumerable<string> roles);
    }
}
