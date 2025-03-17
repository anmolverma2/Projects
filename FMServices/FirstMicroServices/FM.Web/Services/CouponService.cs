using FM.Web.Models;
using FM.Web.Services.IService;
using static FM.Web.Utility.SD;

namespace FM.Web.Services
{
    public class CouponService : ICouponService
    {
        public readonly IBaseService _baseService;
        public CouponService(IBaseService Service)
        {
            _baseService = Service;
        }
        public async Task<ResponseDTO?> CreateCouponAsync(CouponDTO coupon)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = coupon,
                Url = CouponApiBase + "/api/coupon"
            });
        }

        public async Task<ResponseDTO?> DeleteCouponAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.DELETE,
                Url = CouponApiBase + "/api/coupon/" + id
            });
        }

        public async Task<ResponseDTO?> GetAllCouponAsync()
        {
            return await _baseService.SendAsync( new RequestDTO()
            {
                ApiType = ApiType.GET,
                Url = CouponApiBase + "/api/coupon"
            });
        }

        public async Task<ResponseDTO?> GetCouponByIDAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.GET,
                Url = CouponApiBase + "/api/coupon/" + id
            });
        }

        public async Task<ResponseDTO?> UpdateCouponAsync(CouponDTO coupon)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.PUT,
                Data = coupon,
                Url = CouponApiBase + "/api/coupon/"
            });
        }
    }
}
