using System.Threading;
using Auth.Application.Features.Authentication.Login;
using Auth.Application.Features.Authentication.Logout;
using Auth.Application.Features.Authentication.RefreshToken;
using Auth.Application.Features.Authentication.UpdatePassword;
using Auth.Application.Features.Registration;
using BuildingBlocks.Extensions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Auth.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost(
                "/register",
                async (RegisterCommand command, ISender sender, CancellationToken ct) =>
                {
                    var result = await sender.Send(command, ct);
                    return result.IsSuccess ? Results.Created() : result.ToProblemResult();
                }
            )
            .RequireRateLimiting("auth-strict");

        group.MapPost(
                "/login",
                async (LoginCommand command, ISender sender, CancellationToken ct) =>
                {
                    var result = await sender.Send(command, ct);
                    return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemResult();
                }
            )
            .RequireRateLimiting("auth-strict");

        group.MapPost(
                "/refresh-token",
                async (RefreshTokenCommand command, ISender sender, CancellationToken ct) =>
                {
                    var result = await sender.Send(command, ct);
                    return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemResult();
                }
            )
            .RequireRateLimiting("auth-token");

        group
            .MapPatch(
                "/update-password",
                async (UpdatePasswordCommand command, ISender sender, CancellationToken ct) =>
                {
                    var result = await sender.Send(command, ct);
                    return result.IsSuccess ? Results.NoContent() : result.ToProblemResult();
                }
            )
            .RequireAuthorization()
            .RequireRateLimiting("auth-strict");

        group
            .MapPost(
                "/logout",
                async (ISender sender, CancellationToken ct) =>
                {
                    var result = await sender.Send(new LogoutCommand(), ct);
                    return result.IsSuccess ? Results.NoContent() : result.ToProblemResult();
                }
            )
            .RequireAuthorization();

        return app;
    }
}