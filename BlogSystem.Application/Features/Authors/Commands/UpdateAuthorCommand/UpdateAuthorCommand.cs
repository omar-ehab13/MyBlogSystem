using BlogSystem.Application.Interfaces;
using BlogSystem.Domain.Common;
using BlogSystem.Domain.Exceptions;
using BlogSystem.Domain.Repositories;
using MediatR;

namespace BlogSystem.Application.Features.Authors.Commands.UpdateAuthorCommand;

public record UpdateAuthorCommand(Guid Id, UpdateAuthorDto AuthorDto) : IRequest<Result<AuthorDto>>;

public class UpdateAuthorCommandHandler : IRequestHandler<UpdateAuthorCommand, Result<AuthorDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMappingService _mappingService;

    public UpdateAuthorCommandHandler(IUnitOfWork unitOfWork, IMappingService mappingService)
    {
        _unitOfWork = unitOfWork;
        _mappingService = mappingService;
    }

    public async Task<Result<AuthorDto>> Handle(UpdateAuthorCommand request, CancellationToken cancellationToken)
    {
        // 1. Ensure that target author is found and has valid id
        var author = await _unitOfWork.AuthorRepository.GetByIdAsync(request.Id);

        if (author == null)
            throw new NotFoundException($"The author id: {request.Id} not found");

        // 2. Ensure that updated email not exists in DB
        var isEmailExists = await _unitOfWork.AuthorRepository.EmailExistsAsync(request.AuthorDto.Email);

        if (isEmailExists)
            return Result<AuthorDto>.Failure(new() { "Email: email is already exists" },
                409, "Author with this email already exists");

        // 3. Update
        author.UpdateDetails(
            name: request.AuthorDto.Name,
            email: request.AuthorDto.Email,
            imageUrl: request.AuthorDto.ImageUrl);

        // 4. Save in DB
        if (await _unitOfWork.SaveChangesAsync() == 0)
            return Result<AuthorDto>.Failure(new() { "Error: Cannot save updated author in DB" },
                500, "Internal Server Error");

        // 5. Prepare AuthorDto to return with Result
        var authorDto = _mappingService.Map<AuthorDto>(author);

        // 6. Return Result.Success
        return Result<AuthorDto>.Success(authorDto, "Updated", 204);
    }
}
