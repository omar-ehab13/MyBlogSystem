using BlogSystem.Application.DTOs;
using BlogSystem.Domain.Common;
using BlogSystem.Domain.Exceptions;
using BlogSystem.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSystem.Application.Features.Blogs.Queries
{
    public record GetBlogByIdQuery(Guid id) : IRequest<Result<BlogDto>>;

    public class GetBlogByIdHandler : IRequestHandler<GetBlogByIdQuery, Result<BlogDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetBlogByIdHandler(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<Result<BlogDto>> Handle(GetBlogByIdQuery request, CancellationToken cancellationToken)
        {
            var blog = await _unitOfWork.BlogRepository.GetByIdAsync(request.id, cancellationToken);

            if (blog == null)
                throw new NotFoundException("blod is not found");

            var blogDto = new BlogDto(blog.Id, blog.Title, blog.Body, blog.AuthorId, "Ali", blog.CreatedAt, blog.UpdatedAt);

            return Result<BlogDto>.Success(blogDto);
        }
    }
}
