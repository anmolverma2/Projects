using AutoMapper;
using FM.Services.ShoppingCart.Models;
using FM.Services.ShoppingCart.Models.DTO;

namespace FM.Services.CoupenAPI.Configration
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<CartDetails,CartDetailsDto>().ReverseMap();
            CreateMap<CartHeader,CartHeaderDto>().ReverseMap();
        }
    }
}
