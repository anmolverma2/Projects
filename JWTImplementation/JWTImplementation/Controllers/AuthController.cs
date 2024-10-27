using JWTImplementation.Interface;
using JWTImplementation.Models;
using JWTImplementation.RequestModel;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JWTImplementation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // GET: api/<AuthController>
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        // POST api/<AuthController>
        [HttpPost]
        public string Login([FromBody] LoginRequest loginRequest)
        {
            var login = _authService.Login(loginRequest);
            return login;
        }

        // PUT api/<AuthController>/5
        [HttpPost("addUser")]
        public User AddUser([FromBody] User userModel)
        {
            var user = _authService.AddUser(userModel);
            return user;
        }

    }
}
