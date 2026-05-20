using System.Data;

namespace Auth.Application.Abstractions.Persistance;

public interface IUnitOfWork
{
    IDbConnection Connection { get; }
    IDbTransaction Transaction { get; }
    Task CommitAsync();
    void Rollback();
}