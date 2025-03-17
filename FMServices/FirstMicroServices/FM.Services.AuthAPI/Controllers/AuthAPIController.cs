
using FM.Services.AuthAPI.Models.DTO;
using FM.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FM.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        public readonly IAuthService _authService;
        public ResponseModel _response;

        public AuthAPIController(IAuthService authService)
        {
            _authService = authService;
            _response = new();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO user)
        {
            var errorMessage = await _authService.RegisterUser(user);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                _response.IsSuccess = false;
                _response.Message = errorMessage;
                return BadRequest(_response);
            }
            _response.Message = "User Regitered Successfully";
            return Ok(_response);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO requestDTO)
        {
            var user = await _authService.LoginUser(requestDTO);
            if (user.User == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Username or password is invalid";
                return BadRequest(_response);
            }
            _response.IsSuccess = true;
            _response.Data = user;
            return Ok(_response);
        }
        
        [HttpPost("assignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDTO requestDTO)
        {
            var assignRole = await _authService.AssignRole(requestDTO.Email,requestDTO.RoleName.ToUpper());
            if (!assignRole)
            {
                _response.IsSuccess = false;
                _response.Message = "Error during role assign";
                return BadRequest(_response);
            }
            _response.IsSuccess = true;
            _response.Data = requestDTO;
            return Ok(_response);
        }
    }
}
