using System.Data.Common;
using DbUp;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using Testcontainers.PostgreSql;
using Xunit;

namespace PersonalFinance.IntegrationTests;

public class IntegrationTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:17-alpine")
        .WithDatabase("personalfinance")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        
        var connectionString = _dbContainer.GetConnectionString();
        
        RunMigrations(connectionString);
    }

    private void RunMigrations(string connectionString)
    {
        var migrationsPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "database", "migrations");
        
        // Se não encontrar o caminho relativo acima (depende de onde o teste roda), tenta o diretório atual
        if (!Directory.Exists(migrationsPath))
        {
             migrationsPath = Path.Combine(Directory.GetCurrentDirectory(), "database", "migrations");
        }

        var upgrader = DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .WithScriptsFromFileSystem(migrationsPath)
            .LogToConsole()
            .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            throw new Exception("Falha ao executar migrações para testes de integração", result.Error);
        }
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var connectionString = _dbContainer.GetConnectionString();
        
        builder.UseSetting("ConnectionStrings:DefaultConnection", connectionString);
        builder.UseSetting("JWT:Key", "minha-chave-secreta-muito-longa-para-testes-123456");

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbConnection));
            services.RemoveAll(typeof(NpgsqlConnection));

            services.AddSingleton(_ => new NpgsqlConnection(connectionString));

            // Configura Autenticação de Teste
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthHandler.AuthenticationScheme;
                options.DefaultChallengeScheme = TestAuthHandler.AuthenticationScheme;
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                TestAuthHandler.AuthenticationScheme, options => { });
        });
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
}
