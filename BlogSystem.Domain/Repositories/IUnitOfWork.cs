namespace BlogSystem.Domain.Repositories;

public interface IUnitOfWork : IDisposable
{
    IBlogRepository BlogRepository { get; }
    IAuthorRepository AuthorRepository { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
