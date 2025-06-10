using BlogSystem.Application.DTOs;
using BlogSystem.Application.Interfaces;
using BlogSystem.Domain.Common;
using BlogSystem.Domain.Entities;
using BlogSystem.Domain.Exceptions;
using BlogSystem.Domain.Repositories;
using FluentValidation;
using MediatR;

public record CreateBlogCommand(CreateBlogDto BlogDto) : IRequest<Result<BlogDto>>;

public class CreateBlogCommandHandler : IRequestHandler<CreateBlogCommand, Result<BlogDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMappingService _mappingService;

    public CreateBlogCommandHandler(IUnitOfWork unitOfWork, IMappingService mappingService)
    {
        _unitOfWork = unitOfWork;
        this._mappingService = mappingService;
    }

    public async Task<Result<BlogDto>> Handle(CreateBlogCommand request, CancellationToken cancellationToken)
    {
        // Validate author exists
        var authorExists = await _unitOfWork.AuthorRepository.ExistsAsync(request.BlogDto.AuthorId, cancellationToken);

        if (!authorExists)
            throw new ValidationException("author not found");

        var blog = _mappingService.Map<Blog>(request.BlogDto);
        await _unitOfWork.BlogRepository.AddAsync(blog, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Get the created blog with author
        var createdBlog = await _unitOfWork.BlogRepository.GetByIdAsync(blog.Id, cancellationToken);
        var author = await _unitOfWork.AuthorRepository.GetByIdAsync(blog.AuthorId, cancellationToken);

        var blogDto = _mappingService.Map<BlogDto>(blog);

        return Result<BlogDto>.Success(blogDto, "Blog created successfully", 201);
    }
}