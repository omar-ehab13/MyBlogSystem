using BlogSystem.Domain.Exceptions;
using BlogSystem.Domain.Repositories;
using BlogSystem.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BlogSystem.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly BlogSystemDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(BlogSystemDbContext context)
    {
        _context = context;
        BlogRepository = new BlogRepository(_context);
        AuthorRepository = new AuthorRepository(_context);
    }

    public IBlogRepository BlogRepository { get; }
    public IAuthorRepository AuthorRepository { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new DbUpdateConcurrencyException("Data was modified or deleted by another user. Please refresh and try again.", ex);
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx)
            {
                switch (sqlEx.Number)
                {
                    case 515: // Cannot insert the value NULL into column 'X', column does not allow nulls.
                        throw new DatabaseConstraintViolationException($"A required field was missing or invalid: {sqlEx.Message}");
                    case 2601: // Cannot insert duplicate key row in object 'X' with unique index 'Y'. The duplicate key value is (Z).
                    case 2627: // Violation of PRIMARY KEY constraint 'PK_X'. Cannot insert duplicate key in object 'Y'.
                        throw new DuplicateEntryException($"A record with the same unique identifier already exists: {sqlEx.Message}");
                    default:
                        throw new DatabaseOperationException($"An unexpected SQL Server error occurred: {sqlEx.Message} (Error Code: {sqlEx.Number})");
                }
            }
            else
            {
                throw new DatabaseOperationException($"A database error occurred: {ex.Message}");
            }
        }
        catch (TimeoutException)
        {
            throw new DatabaseConnectionException("The database operation timed out.");
        }
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}