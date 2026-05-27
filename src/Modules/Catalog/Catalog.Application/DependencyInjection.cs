using BuildingBlocks.Contracts;
using Catalog.Application.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddCatalogApplication(this IServiceCollection services)
    {
        services.AddScoped<ICatalogModule, CatalogModule>();
        return services;
    }
}