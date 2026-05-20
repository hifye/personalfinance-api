using System.Data;
using Auth.Application.Abstractions.Persistance;

namespace Auth.Infrastructure.Persistance.UnitOfWork;

/// <summary>
/// Implementação do padrão Unit of Work para gerenciar transações de banco de dados.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly IDbConnection _connection;
    
    public IDbTransaction Transaction { get; private set; }
    
    public UnitOfWork(IDbConnection connection)
    {
        _connection = connection;
        if(_connection.State == ConnectionState.Closed)
            _connection.Open();
        Transaction = _connection.BeginTransaction();
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
            Rollback();
            throw;
        }
        finally
        {
            Transaction.Dispose();
            Transaction = _connection.BeginTransaction();
        }
    }
    
    public void Rollback()
    {
        Transaction?.Rollback();
        Transaction?.Dispose();
        Transaction = _connection.BeginTransaction();
    }
    
    public void Dispose()
    {
        Transaction?.Dispose();
        _connection?.Dispose();
    }
}