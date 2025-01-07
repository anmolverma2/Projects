using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;
using PracticeAPI.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PracticeAPI.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpPost]
        public IActionResult Index(LoginDTO model) 
        {
            if (!ModelState.IsValid) {
                ModelState.AddModelError("Message", "Please enter username and password!");
                return BadRequest(ModelState);
            }
            LoginResponseDTO response = new() { Name = model.UserName};

            if (model.UserName == "Anmol" && model.Password == "Anmol@123")
            {
                var key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("JwtBearer:SecretKey"));
              
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor()
                {
                    //   Issuer = issuer,
                    //   Audience = audience,
                 //   Issuer = _configuration.GetValue<string>("JwtBearer:Issuer"),
                   // Audience = _configuration.GetValue<string>("JwtBearer:Audience"),

                    Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name,model.UserName),
                        new Claim(ClaimTypes.Role,"Admin")
                    }),
                    Expires = DateTime.Now.AddDays(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)//
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenResponse = tokenHandler.WriteToken(token);
                response.Token = tokenResponse;
            }
            else
            {
                return Ok("Invalid Credentials!");
            }

            return Ok(response);
        }
    }
}
