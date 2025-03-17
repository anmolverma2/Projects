using FM.Web.Models;
using FM.Web.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;

namespace FM.Web.Controllers
{
    public class HomeController : Controller
    {
        public readonly IProductService _productService;
        public readonly ICartService _cartService;
        public HomeController(IProductService productService, ICartService cartService)
        {
            _productService = productService;
            _cartService = cartService;
        }
        public async Task<IActionResult> Index()
        {
            List<ProductDto> list = new();
            ResponseDTO? responseDTO = await _productService.GetAllProductAsync();
            if (responseDTO != null && responseDTO.IsSuccess)
            {

                TempData["success"] = "Records fetched successfully!";
                list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(responseDTO.Data));
            }
            else
            {
                TempData["error"] = responseDTO?.Message;
            }

            return View(list);
        }
        [Authorize]

        public async Task<IActionResult> Details(int productId)
        {
            ProductDto product = new();
            ResponseDTO? responseDTO = await _productService.GetProductByIDAsync(productId);
            if (responseDTO != null && responseDTO.IsSuccess)
            {

                TempData["success"] = "Records fetched successfully!";
                product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(responseDTO.Data));
            }
            else
            {
                TempData["error"] = responseDTO?.Message;
            }

            return View(product);
        }

        [Authorize]
        [ActionName("Details")]
        [HttpPost]
        public async Task<IActionResult> Details(ProductDto productDto)
        {
            CartDto cartDto = new CartDto()
            {
                cartHeader = new CartHeaderDto()
                {
                    UserId = User.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value
                }
            };
            CartDetailsDto cartDetailsDto = new CartDetailsDto()
            {
                Count = productDto.Count,
                ProductId = productDto.ProductId,
            };

            List<CartDetailsDto> cartDetails = new() { cartDetailsDto };
            cartDto.cartDetails = cartDetails;

            ResponseDTO? responseDTO = await _cartService.UpsertCartAsync(cartDto);

            if (responseDTO != null && responseDTO.IsSuccess)
            {
                TempData["success"] = "Item has been added to shopping cart !!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = responseDTO?.Message;
            }

            return View(productDto);
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
