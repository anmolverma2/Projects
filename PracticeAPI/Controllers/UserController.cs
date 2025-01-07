using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PracticeAPI.Model;
using PracticeAPI.Services;
using System.Net;

namespace PracticeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly APIResponse _response;
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;
        public UserController(IUserService service, ILogger<UserController> logger,IMapper mapper)
        {
            _logger = logger;
            _userService = service;
            _response = new();
            _mapper = mapper;
        }
        [HttpPost("Create")]
        public async Task<ActionResult<APIResponse>> CreateNewUser([FromBody] UserDTO userDTO)
        {
            try
            {
                var response = await _userService.CreateUser(userDTO);
                _response.Data = response;
                _response.Success = true;
                _response.StatusCode = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception ex) {

                _response.Success = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.Error = ex.Message.ToString();    
                return _response;
            }
        }

        [HttpGet("All")]
        public async Task<ActionResult<List<UserDTO>>> GetAllUsers()
        {
            try
            {
                var res = await _userService.GetAllUsers();

                return Ok(_mapper.Map<List<UserDTO>>(res));
            }
            catch (Exception ex)
            {
                return null ;
            }
        }

        [HttpGet("{id:int}",Name = "GetUserById")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            try
            {
                var user = await _userService.GetUserById(id);

                return Ok(_mapper.Map<UserDTO>(user));
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet("{name:alpha}", Name = "GetUserByName")]
        public async Task<ActionResult<UserDTO>> GetUserByName(string name)
        {
            var user = await _userService.GetUserByName(name);
            
            return Ok(_mapper.Map<UserDTO>(user));
        }
        
        [HttpDelete("{id:int}", Name = "DeleteUserById")]
        public async Task<ActionResult<UserDTO>> DeleteUser(int id)
        {
            var user = await _userService.DeleteUserById(id);
            return Ok(user);
        }

        [HttpPut("UpdateUser")]
        public async Task<ActionResult<UserDTO>> UpdateUser([FromBody] UserDTO userdto)
        {
            var user = await _userService.UpdateUser(userdto);
            return Ok(user);
        }
    }
}
