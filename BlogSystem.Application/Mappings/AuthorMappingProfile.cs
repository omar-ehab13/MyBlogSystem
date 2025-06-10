using AutoMapper;
using BlogSystem.Domain.Entities;

namespace BlogSystem.Application.Mappings
{
    public class AuthorMappingProfile : Profile
    {
        public AuthorMappingProfile()
        {
            CreateMap<CreateAuthorDto, Author>().ReverseMap();
            CreateMap<AuthorDto, Author>().ReverseMap();
        }
    }
}
