using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using Xunit;

namespace PersonalFinance.IntegrationTests;

[Collection("Shared collection")]
public abstract class BaseIntegrationTest : IAsyncLifetime
{
    private readonly Func<Task> _resetDatabase;
    protected readonly HttpClient HttpClient;
    protected readonly IServiceProvider ServiceProvider;

    protected BaseIntegrationTest(IntegrationTestFactory factory)
    {
        HttpClient = factory.CreateClient();
        ServiceProvider = factory.Services;

        var connectionString = factory.Services.GetRequiredService<NpgsqlConnection>().ConnectionString;
        _resetDatabase = async () =>
        {
            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();
            
            var checkpoint = await Respawner.CreateAsync(connection, new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = new[] { "auth", "finance", "catalog" }
            });

            await checkpoint.ResetAsync(connection);
        };
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() => await _resetDatabase();
}
