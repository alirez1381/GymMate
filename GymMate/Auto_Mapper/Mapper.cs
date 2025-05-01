using AutoMapper;
using GymMate.Model;
using GymMate.Models;
using GymMate.Models.DTO;

namespace GymMate.Auto_Mapper
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            
            CreateMap<ApplicationUser , UserDTO>().ReverseMap();    

        }
    }
}
