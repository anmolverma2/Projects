using FM.Services.AuthAPI.Data;
using FM.Services.AuthAPI.Models;
using FM.Services.AuthAPI.Models.DTO;
using FM.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FM.Services.AuthAPI.Service
{
    public class AuthService : IAuthService
    {
        public readonly AppDBContext _dBContext;
        public readonly UserManager<ApplicationUser> _userManager;
        public readonly RoleManager<IdentityRole> _roleManager;
        public readonly IJwtTokenGenerator _jwtToken;
        public AuthService(IJwtTokenGenerator jwtToken, AppDBContext dBContext, UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            _dBContext = dBContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtToken = jwtToken;
        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = await _dBContext.ApplicationUsers.FirstOrDefaultAsync(x => x.Email.ToLower() == email.ToLower());
            if (user != null)
            {
                if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;
        }

        public async Task<LoginResposeDTO> LoginUser(LoginRequestDTO requestDTO)
        {
            var user = await _dBContext.ApplicationUsers.FirstOrDefaultAsync(x => x.UserName.ToLower() == requestDTO.UserName.ToLower() );
            
            bool isValid = await _userManager.CheckPasswordAsync(user,requestDTO.Password);
            
            if (user == null || isValid == false)
            {
                return new LoginResposeDTO() { User = null, Token = "" };
            }

            var roles = await _userManager.GetRolesAsync(user);

            var token = _jwtToken.GenerateToken(user,roles);

            UserDTO userDTO = new()
            {
                Email = user.Email,
                ID = user.Id,
                PhoneNumber = user.PhoneNumber,
                Name = user.Name
            };

            LoginResposeDTO loginRespose = new LoginResposeDTO()
            {
                User = userDTO,
                Token = token
            };
            return loginRespose;
        }

        public async Task<string> RegisterUser(RegistrationRequestDTO requestDTO)
        {
            ApplicationUser user = new()
            {
                UserName = requestDTO.Email,
                NormalizedEmail = requestDTO.Email.ToUpper(),
                Email = requestDTO.Email,
                PhoneNumber = requestDTO.PhoneNumber,
                Name = requestDTO.Name
            };
            try
            {
                var result = await _userManager.CreateAsync(user,requestDTO.Password);
                if (result.Succeeded)
                {
                    var userToReturn = await _dBContext.ApplicationUsers.FirstAsync(x => x.UserName == requestDTO.Email);
                    UserDTO userDTO = new()
                    {
                        ID = userToReturn.Id,
                        Email = userToReturn.Email,
                        Name = userToReturn.Name,
                        PhoneNumber = userToReturn.PhoneNumber
                    };

                    return "";
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }
            }
            catch (Exception ex)
            {

            }
            throw new NotImplementedException();
        }

    
    
    }
}
