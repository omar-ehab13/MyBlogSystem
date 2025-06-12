using BlogSystem.Application.Interfaces;
using BlogSystem.Domain.Common;
using BlogSystem.Domain.Entities;
using BlogSystem.Domain.Repositories;
using MediatR;

namespace BlogSystem.Application.Features.Authors.Commands.CreateAuthorCommand;

public record CreateAuthorCommand(CreateAuthorDto AuthorDto) : IRequest<Result<AuthorDto>>;

public class CreateAuthorCommandHandler : IRequestHandler<CreateAuthorCommand, Result<AuthorDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMappingService _mappingService;

    public CreateAuthorCommandHandler(IUnitOfWork unitOfWork, IMappingService mappingService)
    {
        _unitOfWork = unitOfWork;
        _mappingService = mappingService;
    }

    public async Task<Result<AuthorDto>> Handle(CreateAuthorCommand request, CancellationToken cancellationToken)
    {
        var isEmailExists = await _unitOfWork.AuthorRepository.EmailExistsAsync(request.AuthorDto.Email);

        if (isEmailExists)
            return Result<AuthorDto>.Failure(new() { "Email: Author with this email already exists" },
                409, "Author with this email already exists");

        var author = _mappingService.Map<Author>(request.AuthorDto);
        var createdAuthor = await _unitOfWork.AuthorRepository.AddAsync(author);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var authorDto = _mappingService.Map<AuthorDto>(createdAuthor!);

        return Result<AuthorDto>.Success(authorDto, "Created", 201);
    }
}
