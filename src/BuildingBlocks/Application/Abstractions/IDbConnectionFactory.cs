using System.Data;

namespace BuildingBlocks.Application.Abstractions;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}