using FM.Web.Models;
using FM.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FM.Web.Controllers
{
    public class ProductController : Controller
    {
        public readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        public async Task<IActionResult> ProductIndex()
        {
            List<ProductDto> list = new();
            ResponseDTO? responseDTO = await _productService.GetAllProductAsync();
            if (responseDTO != null && responseDTO.IsSuccess) {

                TempData["success"] = "Records fetched successfully!";
                list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(responseDTO.Data));
            }
            else
            {
                TempData["error"] = responseDTO?.Message;
            }

            return View(list);
        }
        [HttpGet]
        public async Task<IActionResult> ProductCreate()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ProductCreate(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                ResponseDTO? response = await _productService.CreateProductAsync(model);
                if(response != null && response.IsSuccess)
                {
                    TempData["success"] = "Records created successfully!";

                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> ProductDelete(int productId)
        {

            ProductDto model = new();
            ResponseDTO? response = await _productService.GetProductByIDAsync(productId);
            if(response != null && response.IsSuccess)
            {
                model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Data));

                return View(model);
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> ProductDelete(ProductDto model)
        {
            ResponseDTO? response = await _productService.DeleteProductAsync(model.ProductId);
            if(response != null && response.IsSuccess)
            {
                TempData["success"] = "Records deleted successfully!";

                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> ProductEdit(int productId)
        {

            ProductDto model = new();
            ResponseDTO? response = await _productService.GetProductByIDAsync(productId);
            if (response != null && response.IsSuccess)
            {
                model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Data));

                return View(model);
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> ProductEdit(ProductDto model)
        {
            ResponseDTO? response = await _productService.UpdateProductAsync(model);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Records updated successfully!";

                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return NotFound();
        }
    }
}
