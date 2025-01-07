using AutoMapper;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using PracticeAPI.Common;
using PracticeAPI.Model;
using System.Security.Cryptography;

namespace PracticeAPI.Services
{
    public class UserService : IUserService
    {
        private readonly ICommonRepository<User> _user;
        private readonly IMapper _mapper;
        public UserService(ICommonRepository<User> user,IMapper mapper)
        {
            _user = user;
            _mapper = mapper;
        }

        public async Task<dynamic> CreateUser(UserDTO userdto)
        {
            var existingUser = await _user.GetByAnyFilter(n => n.Username.Equals(userdto.UserName));
            if (existingUser != null)
            {
                throw new Exception("Username is already taken");
            }
            User user = _mapper.Map<User>(userdto);
            user.IsDeleted = false;
            user.CretedDate = DateTime.Now;
            user.ModifiedDate = DateTime.Now;

            if (!string.IsNullOrEmpty(userdto.Password))
            {
                var passwordHash = CreateHashPassword(userdto.Password);
                user.Password = passwordHash.passwordHash;
                user.PasswordSalt = passwordHash.salt;
            }

            var data = await _user.Create(user);
            return data;
        }

        public (string passwordHash,string salt) CreateHashPassword(string password)
        {
            var salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            var hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password : password,
                salt : salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256/8
            ));

            return (hash, Convert.ToBase64String(salt));
        }


        public async Task<dynamic> GetAllUsers()
        {
            var user = await _user.GetAllAsync();
            if (user == null)
                return null;
            var dto = _mapper.Map<List<UserDTO>>(user);
            return dto;
        }
        public async Task<dynamic> GetUserById(int id)
        {
            var user = await _user.GetByAnyFilter(n => n.Id == id);
            if (user == null)
                return null;
            var dto = _mapper.Map<UserDTO>(user);
            return dto;
        }
        public async Task<dynamic> GetUserByName(string username)
        {
            var user = await _user.GetByAnyFilter(n => n.Username == username);
            if (user == null)
                return null;
            var dto = _mapper.Map<UserDTO>(user);
            return dto;
        }

        public async Task<dynamic> DeleteUserById(int id)
        {
            var user = await _user.GetByAnyFilter(n => n.Id == id);
            if (user == null)
                return null;

            await _user.DeleteAsync(user);

            return _mapper.Map<UserDTO>(user);
        }
        
        public async Task<dynamic> UpdateUser(UserDTO userDTO)
        {
            ArgumentNullException.ThrowIfNull(userDTO,nameof(userDTO));
            var existingUser = await _user.GetByAnyFilter(n => n.Id == userDTO.Id,true);
            if (existingUser == null)
                throw new Exception($"User not found with the id {userDTO.Id}");

            var userToUpdate = _mapper.Map<User>(userDTO);
            userToUpdate.ModifiedDate = DateTime.Now;

            if (!string.IsNullOrEmpty(userDTO.Password))
            {
                var hashPassword = CreateHashPassword(userToUpdate.Password);
                userToUpdate.Password = hashPassword.passwordHash;
                userToUpdate.PasswordSalt = hashPassword.salt;
            }

            await _user.UpdateAsync(userToUpdate);
            return userDTO;
        }

    }
}
