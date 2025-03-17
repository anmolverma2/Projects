using FM.Web.Models;
using FM.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FM.Web.Controllers
{
    public class CouponController : Controller
    {
        public readonly ICouponService _couponService;
        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }
        public async Task<IActionResult> CouponIndex()
        {
            List<CouponDTO> list = new();
            ResponseDTO? responseDTO = await _couponService.GetAllCouponAsync();
            if (responseDTO != null && responseDTO.IsSuccess) {

                TempData["success"] = "Records fetched successfully!";
                list = JsonConvert.DeserializeObject<List<CouponDTO>>(Convert.ToString(responseDTO.Data));
            }
            else
            {
                TempData["error"] = responseDTO?.Message;
            }

            return View(list);
        }
        [HttpGet]
        public async Task<IActionResult> CouponCreate()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CouponCreate(CouponDTO model)
        {
            if (ModelState.IsValid)
            {
                ResponseDTO? response = await _couponService.CreateCouponAsync(model);
                if(response != null && response.IsSuccess)
                {
                    TempData["success"] = "Records created successfully!";

                    return RedirectToAction(nameof(CouponIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> CouponDelete(int couponId)
        {

            CouponDTO model = new();
            ResponseDTO? response = await _couponService.GetCouponByIDAsync(couponId);
            if(response != null && response.IsSuccess)
            {
                model = JsonConvert.DeserializeObject<CouponDTO>(Convert.ToString(response.Data));

                return View(model);
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> CouponDelete(CouponDTO model)
        {
            ResponseDTO? response = await _couponService.DeleteCouponAsync(model.CouponId);
            if(response != null && response.IsSuccess)
            {
                TempData["success"] = "Records deleted successfully!";

                return RedirectToAction(nameof(CouponIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return NotFound();
        }
    }
}
