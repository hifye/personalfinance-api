using Auth.Api.Endpoints;
using Microsoft.AspNetCore.Routing;

namespace Auth.Api;

public static class DependencyInjection
{
    public static IEndpointRouteBuilder MapAuthModule(this IEndpointRouteBuilder app)
    {
        app.MapAuthEndpoints();
        return app;
    }
}