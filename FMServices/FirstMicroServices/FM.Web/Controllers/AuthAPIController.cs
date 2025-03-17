using FM.Web.Models;
using FM.Web.Services.IService;
using FM.Web.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FM.Web.Controllers
{
    public class AuthAPIController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;
        public AuthAPIController(IAuthService authService, ITokenProvider tokenProvider)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDTO login)
        {
            ResponseDTO result = await _authService.LoginAsync(login);

            if(result != null && result.IsSuccess)
            {
                LoginResposeDTO loginResponseDTO = JsonConvert.DeserializeObject<LoginResposeDTO>(Convert.ToString(result.Data));

                await SingInAsync(loginResponseDTO);

                _tokenProvider.SetToken(loginResponseDTO.Token);
               
                return RedirectToAction("Index","Home");
            }
            else
            {
                TempData["error"] = result.Message;
                return View(login);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            var rolelist = new List<SelectListItem>()
            {
                new SelectListItem() { Text = SD.RoleAdmin , Value = SD.RoleAdmin },
                new SelectListItem() { Text = SD.RoleCustomer , Value = SD.RoleCustomer }
            };

            ViewBag.RoleList = rolelist;

            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDTO registration)
        {
            var resonse = await _authService.RegisterAsync(registration);
            ResponseDTO assignRole;

            if(resonse != null && resonse.IsSuccess)
            {
                if (string.IsNullOrEmpty(registration.RoleName)){
                    registration.RoleName = SD.RoleCustomer;
                }
                else
                {
                    assignRole = await _authService.AssignRoleAsync(registration);
                    TempData["success"] = "Registration Successful";
                    return RedirectToAction(nameof(Login));
                }
            }
            else
            {
                TempData["error"] = resonse.Message;
            }
            var rolelist = new List<SelectListItem>()
            {
                new SelectListItem() { Text = SD.RoleAdmin , Value = SD.RoleAdmin },
                new SelectListItem() { Text = SD.RoleCustomer , Value = SD.RoleCustomer }
            };

            ViewBag.RoleList = rolelist;

            return View(registration);
        }

        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            _tokenProvider.ClearToken();
            return RedirectToAction("Index","Home");
        }

        private async Task SingInAsync(LoginResposeDTO requestDTO)
        {
            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.ReadJwtToken(requestDTO.Token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email,
                jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email).Value));

            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name, 
                jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Name).Value));

            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, 
                jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub).Value));

            identity.AddClaim(new Claim(ClaimTypes.Name,
               jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email).Value));


            identity.AddClaim(new Claim(ClaimTypes.Role,
               jwt.Claims.FirstOrDefault(x => x.Type == "role").Value));


            var principle = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,principle);
        }

    }
}
