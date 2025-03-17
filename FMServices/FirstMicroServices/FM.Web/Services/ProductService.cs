using FM.Web.Models;
using FM.Web.Services.IService;
using static FM.Web.Utility.SD;

namespace FM.Web.Services
{
    public class ProductService : IProductService
    {
        public readonly IBaseService _baseService;
        public ProductService(IBaseService Service)
        {
            _baseService = Service;
        }
        public async Task<ResponseDTO?> CreateProductAsync(ProductDto product)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = product,
                Url = ProductApiBase + "/api/product"
            });
        }

        public async Task<ResponseDTO?> DeleteProductAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.DELETE,
                Url = ProductApiBase + "/api/product/" + id
            });
        }

        public async Task<ResponseDTO?> GetAllProductAsync()
        {
            return await _baseService.SendAsync( new RequestDTO()
            {
                ApiType = ApiType.GET,
                Url = ProductApiBase + "/api/product"
            });
        }

        public async Task<ResponseDTO?> GetProductByIDAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.GET,
                Url = ProductApiBase + "/api/product/" + id
            });
        }

        public async Task<ResponseDTO?> UpdateProductAsync(ProductDto product)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.PUT,
                Data = product,
                Url = ProductApiBase + "/api/product/"
            });
        }
    }
}
