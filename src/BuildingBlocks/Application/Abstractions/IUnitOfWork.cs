using System.Data;

namespace BuildingBlocks.Application.Abstractions;

public interface IUnitOfWork
{
    IDbTransaction Transaction { get; }
    Task CommitAsync();
    void Rollback();
}