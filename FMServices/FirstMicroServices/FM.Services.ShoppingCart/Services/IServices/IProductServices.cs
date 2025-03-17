using FM.Services.ShoppingCart.Models.DTO;

namespace FM.Services.ShoppingCart.Services.IServices
{
    public interface IProductServices
    {
        Task<IEnumerable<ProductDto>> GetProductsAsync();
    }
}
