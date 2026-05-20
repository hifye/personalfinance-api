using System.Data;

namespace Auth.Application.Abstractions.Persistance;

public interface IUnitOfWork
{
    IDbTransaction Transaction { get; }
    Task CommitAsync();
    void Rollback();
}