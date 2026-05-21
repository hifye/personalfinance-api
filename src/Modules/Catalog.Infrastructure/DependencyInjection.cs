using Catalog.Application.Abstractions.Persistance;
using Catalog.Application.Abstractions.Queries;
using Catalog.Infrastructure.Persistance.Queries;
using Catalog.Infrastructure.Persistance.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCatalogInfrastructure(
        this IServiceCollection services)
    {
        services.AddScoped<ICatalogRepository, CatalogRepository>();
        services.AddScoped<ICatalogQueries, CatalogQueries>();

        return services;
    }
}