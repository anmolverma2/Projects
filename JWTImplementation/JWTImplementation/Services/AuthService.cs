using JWTImplementation.Context;
using JWTImplementation.Interface;
using JWTImplementation.Models;
using JWTImplementation.RequestModel;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
//using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTImplementation.Services
{
    public class AuthService : IAuthService
    {
        private readonly JWTContext _jwtContext;
        private readonly IConfiguration _configuration;
        public AuthService(JWTContext context,IConfiguration configuration)
        {
            _jwtContext = context;
            _configuration = configuration;

        }
        public User AddUser(User user)
        {
            var addedUser = _jwtContext.Users.Add(user);
            _jwtContext.SaveChanges();
            return addedUser.Entity;
        }

        public string Login(LoginRequest loginRequest)
        {
            if(loginRequest.UserName != null && loginRequest.Password != null)
            {
                var user = _jwtContext.Users.SingleOrDefault(s => s.Email == loginRequest.UserName && s.Password == loginRequest.Password);
                 if(user != null)
                 {
                    var claims = new[] {
                        new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Sub,_configuration["Jwt:Subject"]),
                        new Claim("Id",user.Id.ToString()),
                        new Claim("UserName",user.Name.ToString()),
                        new Claim("Email",user.Email.ToString())

                    };
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signin = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        _configuration["Jwt:Issuer"],
                        _configuration["Jwt:Audience"],
                        claims,
                        expires: DateTime.UtcNow.AddSeconds(20),
                        signingCredentials: signin
                        );
                    var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
                    return jwtToken;
                }
                else
                {
                    return "Invalid Credentials";
                }

            }
            else
            {
                return "Invalid Credentials !";
            }

        }
    
    
    
    
    }
}
