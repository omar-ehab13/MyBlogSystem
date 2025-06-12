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
            var authors = await _unitOfWork.AuthorRepository.GetAllAsync(cancellationToken);

            if (!authors.Any())
                return Result<IEnumerable<AuthorDto>>.Success(new List<AuthorDto>(), "No authors found");

            var authorDtos =  _mappingService.Map<List<AuthorDto>>(authors);

            return Result<IEnumerable<AuthorDto>>
                .Success(authorDtos, "Authors retrieved successfully.");
        }
    }
}
