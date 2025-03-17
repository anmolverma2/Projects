using FM.Services.ShoppingCart.Models;
using FM.Services.ShoppingCart.Services.IServices;
using Newtonsoft.Json;

namespace FM.Services.ShoppingCart.Services
{
    public class CouponService : ICouponService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public CouponService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<CouponDTO> GetCouponCode(string couponCode)
        {
            var client = _httpClientFactory.CreateClient("Coupon");
            var response = await client.GetAsync($"/api/coupon/GetbyCode/{couponCode}");
            var content = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseModel>(content);
            if (resp != null)
            {
                return JsonConvert.DeserializeObject<CouponDTO>(Convert.ToString(resp.Data));
            }
            return new CouponDTO();
        }
    }
}
