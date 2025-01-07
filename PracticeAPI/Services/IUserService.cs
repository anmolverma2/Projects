using PracticeAPI.Model;

namespace PracticeAPI.Services
{
    public interface IUserService
    {
        Task<dynamic> CreateUser(UserDTO userdto);
        Task<dynamic> GetAllUsers();
        Task<dynamic> GetUserById(int id);
        Task<dynamic> GetUserByName(string username);
        Task<dynamic> DeleteUserById(int id);
        Task<dynamic> UpdateUser(UserDTO userDTO);
        (string passwordHash, string salt) CreateHashPassword(string password);
    }
}
