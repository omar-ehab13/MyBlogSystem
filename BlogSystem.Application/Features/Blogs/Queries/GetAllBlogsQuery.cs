using MediatR;
using BlogSystem.Domain.Common;
using BlogSystem.Application.DTOs;
using BlogSystem.Domain.Repositories;
using BlogSystem.Application.Interfaces;

namespace BlogSystem.Application.Features.Blogs.Queries;

public record GetAllBlogsQuery : IRequest<Result<IEnumerable<BlogDto>>>;

public class GetAllBlogsQueryHandler : IRequestHandler<GetAllBlogsQuery, Result<IEnumerable<BlogDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMappingService _mappingService;

    public GetAllBlogsQueryHandler(IUnitOfWork unitOfWork, IMappingService mappingService)
    {
        _unitOfWork = unitOfWork;
        this._mappingService = mappingService;
    }

    public async Task<Result<IEnumerable<BlogDto>>> Handle(GetAllBlogsQuery request, CancellationToken cancellationToken)
    {
        var blogs = await _unitOfWork.BlogRepository.GetBlogsWithAuthorsAsync(cancellationToken);

        var blogsDtos = blogs.Select(b => _mappingService.Map<BlogDto>(b));


        return Result<IEnumerable<BlogDto>>.Success(blogsDtos, "Blogs retrieved successfully");
    }
}
