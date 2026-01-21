using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Data.Common;

namespace LimasIoTDevices.Shared.Data;

public interface IUnitOfWork
{
    IDbConnection Connection { get; }
    IDbTransaction? Transaction { get; }
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    ValueTask DisposeAsync();
    Task ExecuteInTransactionAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public sealed class UnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
{
    private readonly TContext _dbContext;

    private DbConnection? _connection;
    private IDbContextTransaction? _transaction;
    private bool _disposed;

    public UnitOfWork(TContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IDbConnection Connection
    {
        get
        {
            _connection ??= _dbContext.Database.GetDbConnection();
            return _connection;
        }
    }

    public IDbTransaction? Transaction => _transaction?.GetDbTransaction();

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            throw new InvalidOperationException("There is already an active transaction.");
        }

        _transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("There is no active transaction to commit.");
        }

        await _transaction.CommitAsync(cancellationToken);
        await DisposeTransactionAsync();
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            return;
        }

        await _transaction.RollbackAsync(cancellationToken);
        await DisposeTransactionAsync();
    }

    public async Task ExecuteInTransactionAsync(
        Func<CancellationToken, Task> action,
        CancellationToken cancellationToken = default)
    {
        await BeginTransactionAsync(cancellationToken);

        try
        {
            await action(cancellationToken);
            await CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private async Task DisposeTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
        }

        if (_connection != null)
        {
            await _connection.DisposeAsync();
        }

        _disposed = true;
    }
}