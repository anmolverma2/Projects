using Microsoft.AspNetCore.Mvc;
using SchoolMSDataAccess.UnitOfWork;
using SchoolMSDataAccess.Entities.Model;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SchoolMS.Controllers
{
    public class StudentController : Controller
    {
        private IUnitOfWork _service;
        private IWebHostEnvironment _webHostEnvironment;
        public StudentController(IUnitOfWork service,IWebHostEnvironment webHost)
        {
            _service = service;
            _webHostEnvironment = webHost;
        }
        public IActionResult Index(string name, string className)
        {
            if (name == null) {
                name = "";
            }
            if (className == null)
            {
                className = "";
            }
            ViewData["name"] = name;
            ViewData["className"] = className; 

            var studentModels = _service.studentRepository.GetStudentData(name, className);

            return View(studentModels);
        }
        public IActionResult Create()
        {
            StudentModel student = new StudentModel();
            student.subjects = _service.subjectRepository.GetSubjectsData();
            
            return View(student);
        }
        [HttpPost]
        public  IActionResult Create(IFormFile formFile,StudentModel model)
        {
            if (formFile != null) {
                string folder = @"Images\" ;

                folder += Guid.NewGuid() + "_" + formFile.FileName.ToString();
               
                string path = Path.Combine(_webHostEnvironment.WebRootPath,folder);
                using (var stream = new FileStream(path,FileMode.Create)) {

                     formFile.CopyTo(stream);
                }

                model.Image = folder;
            }

            long result = _service.studentRepository.InsertStudentData(model);
            if (result == 1) {

                return RedirectToAction("Index");
            }
            else
            {
            return RedirectToAction("Index");
            }
        }
    }
}
