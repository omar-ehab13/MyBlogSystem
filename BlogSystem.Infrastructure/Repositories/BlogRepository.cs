using BlogSystem.Domain.Entities;
using BlogSystem.Domain.Repositories;
using BlogSystem.Infrastructure.Data;
using BlogSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

public class BlogRepository : GenericRepository<Blog>, IBlogRepository
{
    public BlogRepository(BlogSystemDbContext context) : base(context) { }

    public async Task<IEnumerable<Blog>> GetBlogsByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(b => b.Author)
            .Where(b => b.AuthorId == authorId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Blog>> GetBlogsWithAuthorsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(b => b.Author)
            .ToListAsync(cancellationToken);
    }
}
