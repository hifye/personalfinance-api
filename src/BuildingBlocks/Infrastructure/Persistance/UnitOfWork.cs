using System.Data;
using BuildingBlocks.Application.Abstractions;

namespace BuildingBlocks.Infrastructure.Persistance;

public sealed class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly IDbConnection _connection;
    private IDbTransaction? _transaction;

    public IDbTransaction Transaction
    {
        get => _transaction ?? throw new InvalidOperationException(
            "Transaction not started. Call BeginTransaction first.");
    }

    public UnitOfWork(IDbConnectionFactory factory)
    {
        _connection = factory.CreateConnection();
        _transaction = _connection.BeginTransaction();
    }

    public Task CommitAsync()
    {
        try
        {
            _transaction?.Commit();
            return Task.CompletedTask;
        }
        catch
        {
            _transaction?.Rollback();
            throw;
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = _connection.BeginTransaction();
        }
    }

    public void Rollback()
    {
        _transaction?.Rollback();
        _transaction?.Dispose();
        _transaction = _connection.BeginTransaction();
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _connection.Dispose();
    }
}