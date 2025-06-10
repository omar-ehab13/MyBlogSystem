using BlogSystem.Domain.Entities;

namespace BlogSystem.Domain.Repositories;

public interface IBlogRepository : IGenericRepository<Blog>
{
    Task<IEnumerable<Blog>> GetBlogsByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Blog>> GetBlogsWithAuthorsAsync(CancellationToken cancellationToken = default);
}
