using AutoMapper;
using SecilStoreCase.Application.DTOs;
using SecilStoreCase.Domain.Entities;

namespace SecilStoreCase.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Configuration, ConfigurationDTO>(); 
            CreateMap<ConfigurationCreateDTO, Configuration>();
            CreateMap<ConfigurationUpdateDTO, Configuration>();
        }
    }
}
