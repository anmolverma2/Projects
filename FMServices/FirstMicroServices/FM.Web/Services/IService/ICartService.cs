using FM.Web.Models;

namespace FM.Web.Services.IService
{
    public interface ICartService
    {
        Task<ResponseDTO?> UpsertCartAsync(CartDto cartDto);
        Task<ResponseDTO?> RemoveFromCartAsync(int cartDetailsId);
        Task<ResponseDTO?> GetCartByUserIdAsync(string userId);
        Task<ResponseDTO?> ApplyCouponAsync(CartDto cartDto);
    }
}
