using FM.Web.Models;

namespace FM.Web.Services.IService
{
    public interface IProductService
    {
        Task<ResponseDTO?> GetAllProductAsync();
        Task<ResponseDTO?> GetProductByIDAsync(int id);
        Task<ResponseDTO?> CreateProductAsync(ProductDto product);
        Task<ResponseDTO?> UpdateProductAsync(ProductDto product);
        Task<ResponseDTO?> DeleteProductAsync(int id);
    }
}
