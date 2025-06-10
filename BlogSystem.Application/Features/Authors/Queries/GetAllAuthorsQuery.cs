using BlogSystem.Application.Interfaces;
using BlogSystem.Domain.Common;
using BlogSystem.Domain.Repositories;
using MediatR;

namespace BlogSystem.Application.Features.Authors.Queries
{
    public record GetAllAuthorsQuery() : IRequest<Result<IEnumerable<AuthorDto>>>;

    public class GetAllAutorsQueryHandler : IRequestHandler<GetAllAuthorsQuery, Result<IEnumerable<AuthorDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMappingService _mappingService;

        public GetAllAutorsQueryHandler(IUnitOfWork unitOfWork, IMappingService mappingService)
        {
            this._unitOfWork = unitOfWork;
            this._mappingService = mappingService;
        }

        public async Task<Result<IEnumerable<AuthorDto>>> Handle(GetAllAuthorsQuery request, CancellationToken cancellationToken)
        {
            var authors = await _unitOfWork.AuthorRepository.GetAllAsync();

            var authorsDtos = authors.Select(a => _mappingService.Map<AuthorDto>(a)).ToList();

            return Result<IEnumerable<AuthorDto>>.Success(authorsDtos);
        }
    }
}
