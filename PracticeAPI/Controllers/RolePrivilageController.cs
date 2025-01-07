using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PracticeAPI.Common;
using PracticeAPI.Model;
using System.Net;

namespace PracticeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolePrivilageController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICommonRepository<RolePrivilage> _service;
        private APIResponse _response;

        public RolePrivilageController(ICommonRepository<RolePrivilage> common,IMapper mapper)
        {
            _response = new();
            _mapper = mapper;
            _service = common;
        }

        [HttpPost("Create")]
        public async Task<ActionResult<APIResponse>> Create([FromBody] RolePrivilageDTO dto)
        {
            try
            {

                RolePrivilage role = _mapper.Map<RolePrivilage>(dto);
                role.CreatedDate = DateTime.Now;
              //  role.Isdeleted = false;

                var data = await _service.Create(role);
                dto.Id = data.Id;
                _response.Data = data;
                _response.StatusCode = HttpStatusCode.OK;
                _response.Success = true;

                return (_response);
             //   return CreatedAtRoute("GetRoleById", new { id = role.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.Success = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.Error = ex.Message.ToString();
                return _response;
            }
        }

        [HttpGet("GetAllRolePrivilages")]
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

        [HttpGet("{id:int}", Name = "GetRolePrivilageById")]
        public async Task<ActionResult<APIResponse>> GetRolebyId(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest();

                var role = await _service.GetByAnyFilter(n => n.Id == id);
                if (role == null)
                    return NotFound();

                _response.Data = _mapper.Map<RolePrivilage>(role);
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

        [HttpGet("{name:alpha}", Name = "GetRolePrivilageByName")]
        public async Task<ActionResult<APIResponse>> GetRoleByName(string name)
        {
            try
            {
                var role = await _service.GetByAnyFilter(n => n.RolePrivilegeName.Contains(name));
                if (role == null)
                    return NotFound();
                _response.Data = _mapper.Map<RolePrivilage>(role);
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

        [HttpPut("UpdateRolePrivilage")]
        public async Task<ActionResult<APIResponse>> UpdateRole([FromBody] RolePrivilageDTO roleDTO)
        {
            try
            {
                if (roleDTO == null || roleDTO.Id <= 0)
                    return BadRequest();
                var role = await _service.GetByAnyFilter(n => n.Id == roleDTO.Id);
                if (role == null)
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
        [Route("Delete/{id:int}", Name = "DeletePrivilagesbyId")]
        public async Task<ActionResult<APIResponse>> DeleteRole(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest();
                var role = await _service.GetByAnyFilter(n => n.Id == id);

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

        //[Route("GetPrivilagesByAnyFilter")]
        [HttpGet("GetAllPrivilageByFilter/{roleid:int}",Name = "GetPrivilagesByAnyFilter")]
        public async Task<ActionResult<APIResponse>> GetPrivilagesByAnyFilter(int roleid)
        {
            try
            {

                if (roleid <= 0)
                    return BadRequest();

                var role = await _service.GetAllByFilterAsync(n => n.RoleId == roleid);

                if (role == null || !role.Any())
                    return NotFound();

                var privilage = _mapper.Map<List<RolePrivilageDTO>>(role);
                _response.Data = privilage;
                _response.StatusCode = HttpStatusCode.OK;
                _response.Success = true;
                return Ok(_response);
            }
            catch(Exception ex)
            {
                _response.Success = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.Error = ex.Message.ToString();
                return BadRequest(_response);
            }

        }


    }
}
