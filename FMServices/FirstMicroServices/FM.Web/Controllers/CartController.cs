using FM.Web.Models;
using FM.Web.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace FM.Web.Controllers
{
    public class CartController : Controller
    {
        private ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartDtoBasedOnLogginInUser());
        }
        
        public async Task<IActionResult> Remove(int cartDetilsId)
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDTO? response = await _cartService.RemoveFromCartAsync(Convert.ToInt32(userId));

            if (response == null && response.IsSuccess)
            {
                CartDto cartDto = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Data));
                TempData["success"] = "Cart updated successfully !!";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
        {
            ResponseDTO? response = await _cartService.ApplyCouponAsync(cartDto);

            if (response == null && response.IsSuccess)
            {
                TempData["success"] = "Cart updated successfully !!";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
        {
            cartDto.cartHeader.CouponCode = "";
            ResponseDTO? response = await _cartService.ApplyCouponAsync(cartDto);

            if (response == null && response.IsSuccess)
            {
                TempData["success"] = "Cart updated successfully !!";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }
        private async Task<CartDto> LoadCartDtoBasedOnLogginInUser()
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDTO? response = await _cartService.GetCartByUserIdAsync(userId);

            if (response != null && response.IsSuccess)
            {
                CartDto cartDto = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Data));
                return cartDto;
            }
            return new CartDto();
        }

    }
}
