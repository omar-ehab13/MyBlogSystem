﻿using Microsoft.EntityFrameworkCore.Storage;
using BlogSystem.Domain.Repositories;
using BlogSystem.Infrastructure.Data;

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
        return await _context.SaveChangesAsync(cancellationToken);
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