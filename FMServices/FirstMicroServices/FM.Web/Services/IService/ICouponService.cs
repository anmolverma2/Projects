using FM.Web.Models;

namespace FM.Web.Services.IService
{
    public interface ICouponService
    {
        Task<ResponseDTO?> GetAllCouponAsync();
        Task<ResponseDTO?> GetCouponByIDAsync(int id);
        Task<ResponseDTO?> CreateCouponAsync(CouponDTO coupon);
        Task<ResponseDTO?> UpdateCouponAsync(CouponDTO coupon);
        Task<ResponseDTO?> DeleteCouponAsync(int id);
    }
}
