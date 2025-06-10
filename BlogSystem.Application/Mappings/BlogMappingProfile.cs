using AutoMapper;
using BlogSystem.Application.DTOs;
using BlogSystem.Domain.Entities;

namespace BlogSystem.Application.Mappings
{
    public class BlogMappingProfile : Profile
    {
        public BlogMappingProfile()
        {
            CreateMap<Blog, BlogDto>()
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.Name))
                .ReverseMap();
        }
    }
}
