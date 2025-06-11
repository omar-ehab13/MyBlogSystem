using BlogSystem.Application.Interfaces;
using BlogSystem.Domain.Common;
using BlogSystem.Domain.Exceptions;
using BlogSystem.Domain.Repositories;
using MediatR;

namespace BlogSystem.Application.Features.Authors.Commands.DeleteAuthorCommand;

public record DeleteAuthorCommand(Guid Id) : IRequest<Result<object>>;

public class DeleteAuthorCommandHandler : IRequestHandler<DeleteAuthorCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMappingService _mappingService;

    public DeleteAuthorCommandHandler(IUnitOfWork unitOfWork, IMappingService mappingService)
    {
        this._unitOfWork = unitOfWork;
        this._mappingService = mappingService;
    }

    public async Task<Result<object>> Handle(DeleteAuthorCommand request, CancellationToken cancellationToken)
    {
        // 1. check if author exists
        var isAuthorExist = await _unitOfWork.AuthorRepository.ExistsAsync(request.Id);

        if (isAuthorExist)
            throw new NotFoundException("The author already not found in DB");

        // 2. Get target author
        var author = await _unitOfWork.AuthorRepository.GetByIdAsync(request.Id);

        // 3. delete it
        await _unitOfWork.AuthorRepository.DeleteAsync(author!);

        // 4. Save in DB
        if (await _unitOfWork.SaveChangesAsync() == 0)
            return Result<object>.Failure(new() { "DB Error: Cannot deleting author from DB" },
                500, "Internal Server Error");

        // 5. success result
        return Result<object>.Success(author!, "Deleted", 204);
    }
}
