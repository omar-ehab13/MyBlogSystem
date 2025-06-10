using BlogSystem.Domain.Entities;
using BlogSystem.Domain.Repositories;
using BlogSystem.Infrastructure.Data;
using BlogSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

public class AuthorRepository : GenericRepository<Author>, IAuthorRepository
{
    public AuthorRepository(BlogSystemDbContext context) : base(context) { }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(a => a.Email == email, cancellationToken);
    }

    public async Task<Author?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(a => a.Email == email)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
