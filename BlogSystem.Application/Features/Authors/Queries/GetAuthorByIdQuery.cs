using BlogSystem.Application.Interfaces;
using BlogSystem.Domain.Common;
using BlogSystem.Domain.Exceptions;
using BlogSystem.Domain.Repositories;
using MediatR;

namespace BlogSystem.Application.Features.Authors.Queries
{
    public record GetAuthorByIdQuery(Guid Id) : IRequest<Result<AuthorDto>>;

    public class GetAuthorByIdQueryHandler : IRequestHandler<GetAuthorByIdQuery, Result<AuthorDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMappingService _mappingService;

        public GetAuthorByIdQueryHandler(IUnitOfWork unitOfWork, IMappingService mappingService)
        {
            this._unitOfWork = unitOfWork;
            this._mappingService = mappingService;
        }
        public async Task<Result<AuthorDto>> Handle(GetAuthorByIdQuery request, CancellationToken cancellationToken)
        {
            var author = await _unitOfWork.AuthorRepository.GetByIdAsync(request.Id);

            if (author is null)
                throw new NotFoundException("Author not found");

            var authorDto = _mappingService.Map<AuthorDto>(author);

            return Result<AuthorDto>.Success(authorDto, "Author retrieved successfully");
        }
    }
}
