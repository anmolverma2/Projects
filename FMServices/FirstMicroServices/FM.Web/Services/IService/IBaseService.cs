using FM.Services.CoupenAPI.Models;
using FM.Web.Models;

namespace FM.Web.Services.IService
{
    public interface IBaseService
    {
        Task<ResponseDTO?> SendAsync(RequestDTO responseModel,bool isWithBearer = true);
    }
}
