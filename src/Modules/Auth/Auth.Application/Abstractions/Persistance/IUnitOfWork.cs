using System.Data;
using System.Threading.Tasks;

namespace Auth.Application.Abstractions.Persistance;

public interface IUnitOfWork
{
    IDbTransaction Transaction { get; }
    Task CommitAsync();
    void Rollback();
}