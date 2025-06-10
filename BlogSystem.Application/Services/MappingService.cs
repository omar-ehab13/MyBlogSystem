using AutoMapper;
using BlogSystem.Application.Interfaces;

namespace BlogSystem.Application.Services
{
    public class MappingService : IMappingService
    {
        private readonly IMapper _mapper;

        public MappingService(IMapper mapper)
        {
            this._mapper = mapper;
        }

        public TDestination Map<TDestination>(object source)
        {
            return _mapper.Map<TDestination>(source);
        }

        public TDestination Map<TSource, TDestination>(TSource source)
        {
            return _mapper.Map<TSource, TDestination>(source);
        }

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            return _mapper.Map(source, destination);
        }

        public List<TDestination> MapList<TSource, TDestination>(IEnumerable<TSource> source)
        {
            return _mapper.Map<List<TDestination>>(source);
        }
    }
}
