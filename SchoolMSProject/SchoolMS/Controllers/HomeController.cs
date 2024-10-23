using Microsoft.AspNetCore.Mvc;
using SchoolMS.Models;
using SchoolMSDataAccess.UnitOfWork;
using System.Diagnostics;

namespace SchoolMS.Controllers
{
    public class HomeController : Controller
    {
        private IUnitOfWork _service;
       
        public HomeController(IUnitOfWork service)
        {
             _service = service;
        }

        public IActionResult Index()
        {
            TempData["Message"] = "";

            var details = _service.detailRepository.GetDetails();

            return View(details);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
