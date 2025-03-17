using FM.Services.ShoppingCart.Models;
using FM.Services.ShoppingCart.Models.DTO;

namespace FM.Services.ShoppingCart.Services.IServices
{
    public interface ICouponService
    {
        Task<CouponDTO> GetCouponCode(string couponCode);
    }
}
