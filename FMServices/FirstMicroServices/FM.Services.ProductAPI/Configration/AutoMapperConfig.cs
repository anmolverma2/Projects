using AutoMapper;
using FM.Services.ProductAPI.Models;
using FM.Services.ProductAPI.Models.Dto;

namespace FM.Services.ProductAPI.Configration
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<Product,ProductDto>().ReverseMap();
        }
    }
}
