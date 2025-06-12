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
        var author = await _unitOfWork.AuthorRepository.GetByIdAsync(request.Id);

        if (author is null)
            throw new NotFoundException("The author already not found in DB");

        // 2. delete it
        await _unitOfWork.AuthorRepository.DeleteAsync(author!);

        // 4. Save in DB
        await _unitOfWork.SaveChangesAsync();

        // 5. success result
        return Result<object>.Success(author!, "Deleted", 204);
    }
}
