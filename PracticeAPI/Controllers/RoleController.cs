using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PracticeAPI.Common;
using PracticeAPI.Model;
using System.Net;

namespace PracticeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICommonRepository<Role> _service;
        private APIResponse _response;
        public RoleController(IMapper mapper,ICommonRepository<Role> common)
        {
            _response = new();
            _mapper = mapper;
            _service = common;
        }
        
        [HttpPost("Create")]
        public async Task<ActionResult<APIResponse>> Create([FromBody]RoleDTO dto)
        {
            try
            {

                Role role = _mapper.Map<Role>(dto);
                role.CreatedDate = DateTime.Now;
                role.Isdeleted = false;

                var data = await _service.Create(role);
                dto.Id = data.Id;
                _response.Data = data;
                _response.StatusCode = HttpStatusCode.OK;
                _response.Success = true;

                return CreatedAtRoute("GetRoleById", new { id = role.Id }, _response.Data);
            }
            catch (Exception ex) { 
                _response.Success = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.Error = ex.Message.ToString();
                return _response;
            }
        }
     
        [HttpGet("GetAllRoles")]
        public async Task<ActionResult<APIResponse>> GetRoles()
        {
            try
            {
                var roles = await _service.GetAllAsync();
                _response.Data = _mapper.Map<List<Role>>(roles);
                _response.StatusCode = HttpStatusCode.OK;
                _response.Success = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Success = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.Error = ex.Message.ToString();
                return _response;
            }
        }

        [HttpGet("{id:int}",Name = "GetRoleById")]
        public async Task<ActionResult<APIResponse>> GetRolebyId(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest();

                var role = await _service.GetByAnyFilter(n => n.Id == id);
                if(role == null)
                    return NotFound();

                _response.Data = _mapper.Map<Role>(role);
                _response.StatusCode = HttpStatusCode.OK;
                _response.Success= true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Success = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.Error = ex.Message.ToString();
                return _response;
            }
        }
        
        [HttpGet("{name:alpha}",Name = "GetRoleByName")]
        public async Task<ActionResult<APIResponse>> GetRoleByName(string name)
        {
            try
            {
                var role = await _service.GetByAnyFilter(n => n.RoleName == name);
                if(role == null)
                    return NotFound();
                _response.Data = _mapper.Map<Role>(role);
                _response.StatusCode = HttpStatusCode.OK;
                _response.Success = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Success = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.Error = ex.Message.ToString();
                return _response;
            }
        }

        [HttpPut("UpdateRole")]
        public async Task<ActionResult<APIResponse>> UpdateRole([FromBody] RoleDTO roleDTO)
        {
            try
            {
                if (roleDTO == null || roleDTO.Id <= 0)
                    return BadRequest();
                var role = await _service.GetByAnyFilter(n => n.Id == roleDTO.Id);
                if(role == null)
                    return NotFound($"Role not found with id {roleDTO.Id}");    
                //var newRole = _mapper.Map<Role>(roleDTO);
                _mapper.Map(roleDTO, role);

                await _service.UpdateAsync(role);

                _response.Data = roleDTO;
                _response.StatusCode = HttpStatusCode.OK;
                _response.Success = true;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Success = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.Error = ex.Message.ToString();
                return _response;
            }
        }

        [HttpDelete]
        [Route("Delete/{id:int}",Name = "DeletebyId")]
        public async Task<ActionResult<APIResponse>> DeleteRole(int id)
        {
            try
            {
                if(id <= 0)
                    return BadRequest();
                var role = await _service.GetByAnyFilter(n => n.Id ==  id);

                if (role == null)
                    return NotFound();
                await _service.DeleteAsync(role);
                _response.Data = role;
                _response.Success = true;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);

            }
            catch (Exception ex)
            {
                _response.Success = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.Error = ex.Message.ToString();
                return _response;
            }

        }

    }
}
