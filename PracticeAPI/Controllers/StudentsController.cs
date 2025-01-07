using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PracticeAPI.Common;
using PracticeAPI.Context;
using PracticeAPI.Model;
using PracticeAPI.Repositories;
using System.Net;
using System.Text.Json;

namespace PracticeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [EnableCors(PolicyName = "MyPolicy")]
    //   [Authorize(AuthenticationSchemes = "LocatJwt",Roles = "Admin")]
    [Authorize]
    public class StudentsController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly ICollegeRepository _service;
        private readonly IConfiguration _config;
        private APIResponse _response;
        // private readonly ICommonRepository<CollegeStudent> _service;
        public StudentsController(ILogger logger,IMapper mapper, ICollegeRepository repository,IConfiguration configuration)
        {
            _logger = logger;
            _mapper = mapper;
            _service = repository;
            _config = configuration;
            _response = new APIResponse();
        }
        [HttpGet]
        public ActionResult Index()
        {
            _logger.Log(LogLevel.Error, new EventId(1, "TestEvent"), "This is a test message", new Exception("Test exception"), (state, ex) => $"{state} - {ex?.Message}");
            _logger.Log(LogLevel.Error, new EventId(1, "TestEvent"), "This is a test2 log message", new Exception("Test exception"), (state, ex) => $"{state} - {ex?.Message}");

            _logger.LogWarning("This is first warning");
            _logger.LogCritical("this critical log can lead to shit");

            var JwtSecretkey = _config.GetValue<string>("JwtBearer:SecretKey");
            if (string.IsNullOrEmpty(JwtSecretkey)) {
                return BadRequest("JWT secret key is not configured properly.");
            }

            return Ok(new { message = "JWT secret key fetched successfully", JwtSecretkey });
        }


       // [AllowAnonymous]
        [Route("All", Name = "All Student")]
        [HttpGet]
        public async Task<ActionResult<List<APIResponse>>> GetStudents()
        {
            try
            {
                var students = await _service.GetAllAsync();

                _response.Data = _mapper.Map<List<StudentDTO>>(students);
                _response.StatusCode = HttpStatusCode.OK;
                _response.Success = true;

              //  var response = JsonSerializer.Serialize(students);
                return Ok(_response);
            }
            catch (Exception ex) { 
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id:int}", Name = "One student")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<APIResponse>> GetStudentById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Wrong data");
            }
            var student = await _service.GetByAnyFilter(student => student.Id == id);

            if (student == null)
                return NotFound($"student with {id} not available");

            _response.Data = _mapper.Map<StudentDTO>(student);
            _response.StatusCode = HttpStatusCode.OK;
            _response.Success = true;

            return Ok(_response);
        }

        [HttpGet("{name:alpha}", Name = "GetStudentByName")]
        public async Task<ActionResult<APIResponse>> GetStudentByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest();
            }
            var student = await _service.GetByAnyFilter(student => student.Name.ToLower().Contains(name.ToLower()));
            if (student == null)
                return NotFound();
            _response.Data = _mapper.Map<StudentDTO>(student);
            _response.StatusCode = HttpStatusCode.OK;
            _response.Success = true;

            return Ok(_response);
        }

        [HttpDelete("{id:int}", Name = "Delete")]
        public async Task<ActionResult<string>> DeleteStudentById(int id)
        {
            if (id <= 0)
                return BadRequest();
            CollegeStudent student = await _service.GetByAnyFilter(student => student.Id == id);
            if (student == null)
                return NotFound();

            await _service.DeleteAsync(student);

            _response.Data = _mapper.Map<StudentDTO>(student);
            _response.StatusCode = HttpStatusCode.OK;
            _response.Success = true;

            return Ok(new { message = $"Student deleted successfully with ID {id}", response = _response });

        }
        [HttpPost("Create")]
        [DisableCors]
        public async Task<ActionResult<APIResponse>> CreateStudent([FromBody]StudentDTO student)
        {
            if (student == null)
                return BadRequest();

            
            if (student.Age > 100 || student.Age <= 0)
            {
                ModelState.AddModelError("Age Error", "Age is invalid please enter valid age");
                return BadRequest(ModelState);
            }


            var std = _mapper.Map<CollegeStudent>(student);
         //   std.Id = newId; // Ensure the new ID is set

            // Add to the database
            var data = await _service.Create(std);
            // Update the student DTO with the new Id and return it
            //            student.Id = std.Id;


            // CollegeStudent std = new CollegeStudent
            // {
            ////     Id = newId,
            //     Name = student.Name,
            //     Age = student.Age,
            //     Mobile = student.Mobile,
            //     Email = student.Email,
            //     Address = student.Address,
            //     DOB = student.DOB
            // };

           // var lastStudent = await _dbcontext.collegeStudents.OrderByDescending(c => c.Id).FirstOrDefaultAsync();
           // var newId = (lastStudent?.Id ?? 0);
            student.Id = data.Id;
            _response.Data = data;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Success = true;

            return CreatedAtRoute("One student", new {id = student.Id} , _response);
        }

        [HttpPut("Update")]
        public async Task<ActionResult<APIResponse>> UpdateStudent([FromQuery]int id,[FromBody] StudentDTO student)
        {
            if(student == null)
                return BadRequest();

            var exists = await _service.GetByAnyFilter(student => student.Id == id);
            if (exists == null)
            {
                ModelState.AddModelError("Update Error", $"Student with id {id} does not exists.");
                return BadRequest(ModelState);
            }

            student.Id = id;
            _mapper.Map(student, exists);
          //  var updatedStudentDto = _mapper.Map<CollegeStudent>(student);

            await _service.UpdateAsync(exists);
            //   _mapper.Map(student, exists);  // This updates the existing entity with the values from the DTO

            // Save changes to the database
            //  await _dbcontext.SaveChangesAsync();

            // Return the updated StudentDTO

            //exists.Name = student.Name;
            //exists.Age = student.Age;
            //exists.Mobile = student.Mobile;
            //exists.Email = student.Email;
            //exists.Address = student.Address;
            //exists.DOB = student.DOB;
            _response.Data = student;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Success = true;

            return CreatedAtRoute("One student", new {id = id}, _response);
        }


    }
}
