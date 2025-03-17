using AutoMapper;
using FM.Services.CoupenAPI.Models;
using FM.Services.CoupenAPI.Models.DTO;

namespace FM.Services.CoupenAPI.Configration
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<CouponModel,CouponDTO>().ReverseMap();
        }
    }
}
