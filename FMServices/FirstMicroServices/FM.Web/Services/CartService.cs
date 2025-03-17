using FM.Web.Models;
using FM.Web.Services.IService;
using static FM.Web.Utility.SD;

namespace FM.Web.Services
{
    public class CartService : ICartService
    {
        public readonly IBaseService _baseService;
        public CartService(IBaseService Service)
        {
            _baseService = Service;
        }

        public async Task<ResponseDTO?> ApplyCouponAsync(CartDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = cartDto,
                Url = ShoppingCartApiBase + "/api/cart/ApplyCoupon"
            });

        }

        public async Task<ResponseDTO?> GetCartByUserIdAsync(string userId)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = userId,
                Url = ShoppingCartApiBase + "/api/cart/GetCart/"+userId
            });

        }

        public async Task<ResponseDTO?> RemoveFromCartAsync(int cartDetailsId)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = cartDetailsId,
                Url = ShoppingCartApiBase + "/api/cart/RemoveCart/"
            });

        }

        public async Task<ResponseDTO?> UpsertCartAsync(CartDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = cartDto,
                Url = ShoppingCartApiBase + "/api/cart/Upsert"
            });

        }



    }
}
