using Finance.Api.Endpoints;
using Microsoft.AspNetCore.Routing;

namespace Finance.Api;

public static class DependencyInjection
{
    public static IEndpointRouteBuilder MapFinanceModule(this IEndpointRouteBuilder app)
    {
        app.MapFinanceEndpoints();
        return app;
    }
}