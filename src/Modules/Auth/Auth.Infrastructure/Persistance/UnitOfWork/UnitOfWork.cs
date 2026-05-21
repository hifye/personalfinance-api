using System.Data;
using System.Threading.Tasks;
using Auth.Application.Abstractions.Persistance;
using Auth.Infrastructure.Persistance.Connection;

namespace Auth.Infrastructure.Persistance.UnitOfWork;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly IDbConnection _connection;

    public IDbTransaction Transaction { get; }

    public UnitOfWork(IDbConnection connection)
    {
        _connection = connection;

        if (_connection.State == ConnectionState.Closed)
        {
            _connection.Open();
        }

        Transaction =
            _connection.BeginTransaction();
    }

    public Task CommitAsync()
    {
        try
        {
            Transaction.Commit();

            return Task.CompletedTask;
        }
        catch
        {
            Transaction.Rollback();

            throw;
        }
    }

    public void Rollback()
    {
        Transaction.Rollback();
    }

    public void Dispose()
    {
        Transaction.Dispose();

        _connection.Dispose();
    }
}