using FM.Services.ShoppingCart.Models.DTO;
using FM.Services.ShoppingCart.Services.IServices;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using FM.Services.ShoppingCart.Models;

namespace FM.Services.ShoppingCart.Services
{
    public class ProductServices : IProductServices
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ProductServices(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IEnumerable<ProductDto>> GetProductsAsync()
        {
            var client = _httpClientFactory.CreateClient("Product");
            var response = await client.GetAsync($"/api/product");
            var content = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseModel>(content);
            if (resp != null )
            {
                return JsonConvert.DeserializeObject<IEnumerable<ProductDto>>(Convert.ToString(resp.Data)); 
            }
            return new List<ProductDto>();
        }
    }
}
