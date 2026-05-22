using Catalog.Api.Endpoints;
using Microsoft.AspNetCore.Routing;

namespace Catalog.Api;

public static class DependencyInjection
{
    public static IEndpointRouteBuilder MapCatalogModule(this IEndpointRouteBuilder app)
    {
        app.MapCatalogEndpoints();
        return app;
    }
}