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
            throw new FluentValidation.ValidationException("Email is already exists");

        var author = _mappingService.Map<Author>(request.AuthorDto);
        await _unitOfWork.AuthorRepository.AddAsync(author);

        await _unitOfWork.SaveChangesAsync();

        var createdAuthor = await _unitOfWork.AuthorRepository.GetByEmailAsync(author.Email);
        var authorDto = _mappingService.Map<AuthorDto>(createdAuthor!);

        return Result<AuthorDto>.Success(authorDto, "Created");
    }
}
