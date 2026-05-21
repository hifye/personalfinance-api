using System.Data;

namespace Auth.Infrastructure.Persistance.Connection;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}