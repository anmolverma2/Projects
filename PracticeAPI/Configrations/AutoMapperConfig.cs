using AutoMapper;
using PracticeAPI.Model;

namespace PracticeAPI.Configrations
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            //CreateMap<CollegeStudent, StudentDTO>();
            //CreateMap<StudentDTO, CollegeStudent>();
            CreateMap<CollegeStudent, StudentDTO>().ReverseMap();
            CreateMap<RoleDTO,Role>().ReverseMap();
            CreateMap<RolePrivilageDTO,RolePrivilage>().ReverseMap();
            CreateMap<UserDTO,User>().ReverseMap();

        }
    }
}
