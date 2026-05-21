using BuildingBlocks.Extensions;
using Catalog.Application.Features.Commands.CreateCategory;
using Catalog.Application.Features.Commands.DeleteCategory;
using Catalog.Application.Features.Commands.PatchCategory;
using Catalog.Application.Features.Queries.GetCategoriesByUserId;
using Catalog.Application.Features.Queries.GetCategoryDetails;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Catalog.Api.Endpoints;

public static class CatalogEndpoints
{
    public static IEndpointRouteBuilder MapCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/catalog").WithTags("Catalog");

        group.MapPost(
            "/create-category",
            async (CreateCategoryCommand command, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(command, ct);
                return result.IsSuccess ? Results.Created() : result.ToProblemResult();
            }
        )
        .RequireAuthorization();

        group.MapDelete(
            "/delete-category/{id:guid}",
            async (Guid id, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new DeleteCategoryCommand(id), ct);
                return result.IsSuccess ? Results.NoContent() : result.ToProblemResult();
            }
        )
        .RequireAuthorization();

        group.MapPatch(
            "/patch-category/{id:guid}",
            async (Guid id, PatchCategoryCommand command, ISender sender, CancellationToken ct) =>
            {
                command = command with { Id = id };
                var result = await sender.Send(command, ct);
                return result.IsSuccess ? Results.NoContent() : result.ToProblemResult();
            }
        )
        .RequireAuthorization();
        
        group.MapGet(
            "/get-category/{id:guid}",
            async (Guid id, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetCategoryDetailsQuery(id), ct);
                return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemResult();
            }
        )
        .RequireAuthorization();
        
        group.MapGet(
                "/get-categories-user", 
            async (ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetCategoriesByUserIdQuery(), ct);
                return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemResult();
            }
        )
        .RequireAuthorization();

        return app;
    }
}
