using BlogSystem.Application.Interfaces;
using BlogSystem.Domain.Common;
using BlogSystem.Domain.Entities;
using BlogSystem.Domain.Repositories;
using MediatR;

namespace BlogSystem.Application.Features.Authors.Commands;

public record CreateAuthorCommand(CreateAuthorDto AuthorDto) : IRequest<Result<AuthorDto>>;

public class CreateAuthorCommandHandler : IRequestHandler<CreateAuthorCommand, Result<AuthorDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMappingService _mappingService;

    public CreateAuthorCommandHandler(IUnitOfWork unitOfWork, IMappingService mappingService)
    {
        this._unitOfWork = unitOfWork;
        this._mappingService = mappingService;
    }

    public async Task<Result<AuthorDto>> Handle(CreateAuthorCommand request, CancellationToken cancellationToken)
    {
        var authorExists = await _unitOfWork.AuthorRepository.EmailExistsAsync(request.AuthorDto.Email);

        if (authorExists)
            return Result<AuthorDto>.Failure(new() { "Email: Author with this email already exists" },
                409, "Author with this email already exists");

        var author = _mappingService.Map<Author>(request.AuthorDto);
        var createdAuthor = await _unitOfWork.AuthorRepository.AddAsync(author);

        if (await _unitOfWork.SaveChangesAsync() == 0)
            return Result<AuthorDto>.Failure(
                new() { "Error: could not retrieve created author afeter saving" },
                500, "Failed to retrieve created author.");

        var authorDto = _mappingService.Map<AuthorDto>(createdAuthor!);

        return Result<AuthorDto>.Success(authorDto, "Created", 201);
    }
}
