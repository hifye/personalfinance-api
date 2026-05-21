using System.Data;
using BuildingBlocks.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace BuildingBlocks.Infrastructure.Persistance;

internal sealed class DbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
                            ?? throw new InvalidOperationException("DefaultConnection not found in configuration.");
    }

    public IDbConnection CreateConnection()
    {
        var connection = new NpgsqlConnection(_connectionString);
        connection.Open();
        return connection;
    }
}
