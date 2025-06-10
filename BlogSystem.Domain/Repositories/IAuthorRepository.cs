using BlogSystem.Domain.Entities;

namespace BlogSystem.Domain.Repositories;

public interface IAuthorRepository : IGenericRepository<Author>
{
    Task<Author?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}
