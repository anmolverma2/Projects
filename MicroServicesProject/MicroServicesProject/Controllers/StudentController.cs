using MediatR;
using MicroServicesProject.Commands;
using MicroServicesProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MicroServicesProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : BaseController
    {
        [HttpGet]
        [ProducesDefaultResponseType] 
        [Route("StudentDetails")]
        public async Task<ActionResult<ResponseModel>> GetStudentDetails([FromQuery] GetStudentsQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

        [HttpGet]
        [Route("AllDetails")]
        public async Task<ActionResult<ResponseModel>> GetAllDetails([FromQuery] GetAllDetailsQuery query)
        {
            return Ok(await Mediator.Send(query));
        }
    }
}
