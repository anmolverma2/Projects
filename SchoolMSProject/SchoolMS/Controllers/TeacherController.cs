using Microsoft.AspNetCore.Mvc;
using SchoolMSDataAccess.Entities.Model;
using SchoolMSDataAccess.UnitOfWork;
using System.Data;
using System.Reflection;

namespace SchoolMS.Controllers
{
    public class TeacherController : Controller
    {
        private IUnitOfWork _service;
        private IWebHostEnvironment _webHostEnvironment;
        public TeacherController(IUnitOfWork service, IWebHostEnvironment webHost)
        {
            _service = service;
            _webHostEnvironment = webHost;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(IFormFile formFile,TeacherModel model)
        {
            if (formFile != null)
            {
                string folder = @"Images\";

                folder += Guid.NewGuid() + "_" + formFile.FileName.ToString();

                string path = Path.Combine(_webHostEnvironment.WebRootPath, folder);
                using (var stream = new FileStream(path, FileMode.Create))
                {

                    formFile.CopyTo(stream);
                }

                model.ImagePath = folder;
            }

            long result = _service.teacherRepository.InsertTeacherData(model);
            if (result == 1)
            {
                TempData["Message"] = "Record Created Successfully";
                return Redirect("/Home/Index");
            }
            else
            {
                TempData["Message"] = "Error occured";
                return Redirect("/Home/Index");
            }
        }


        public IActionResult AddSubject()
        {
            SubjectModel model = new SubjectModel();
            model.teachers = _service.subjectRepository.GetTeacherData();
            return View(model);
        }
        [HttpPost]
        public IActionResult AddSubject(SubjectModel model)
        {
            model.teachers = _service.subjectRepository.GetTeacherData();
            long result = _service.subjectRepository.InsertSubject(model);
            if (result == 1)
            {
                TempData["Message"] = "Record Created Successfully";
                return Redirect("/Home/Index");
            }
            else
            {

                TempData["Message"] = "Error occured";
                return Redirect("/Home/Index");
            }
        }

    }
}
