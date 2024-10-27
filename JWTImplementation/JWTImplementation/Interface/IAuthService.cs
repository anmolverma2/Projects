using JWTImplementation.Models;
using JWTImplementation.RequestModel;

namespace JWTImplementation.Interface
{
    public interface IAuthService
    {
        User AddUser(User user);
        string Login(LoginRequest loginRequest);
    }
}
