using BuildingBlocks.Extensions;
using Finance.Application.Features.Commands.Account.CreateAccount;
using Finance.Application.Features.Commands.Account.DeleteAccount;
using Finance.Application.Features.Commands.Account.PatchAccount;
using Finance.Application.Features.Commands.RecurringTransaction.CreateRecurringTransaction;
using Finance.Application.Features.Commands.RecurringTransaction.DeleteRecurringTransaction;
using Finance.Application.Features.Commands.RecurringTransaction.PatchRecurringTransaction;
using Finance.Application.Features.Commands.Transaction.CreateTransaction;
using Finance.Application.Features.Commands.Transaction.DeleteTransaction;
using Finance.Application.Features.Commands.Transaction.PatchTransaction;
using Finance.Application.Features.Queries.Account.GetAccountDetails;
using Finance.Application.Features.Queries.Account.GetAccountsByUserId;
using Finance.Application.Features.Queries.RecurringTransaction.GetRecurringTransactionDetails;
using Finance.Application.Features.Queries.RecurringTransaction.GetRecurringTransactionsByUserId;
using Finance.Application.Features.Queries.Transaction.GetTransactionDetails;
using Finance.Application.Features.Queries.Transaction.GetTransactionsByUserId;
using Finance.Application.Features.Queries.Transaction.GetTransactionSummary;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Finance.Api.Endpoints;

public static class FinanceEndpoints
{
    public static IEndpointRouteBuilder MapFinanceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/finance").WithTags("Finance");

        group.MapGet(
            "/get-account-details/{id:guid}",
            async (Guid id, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetAccountDetailsQuery(id), ct);
                return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemResult();
            }
        )
        .RequireAuthorization()
        .RequireRateLimiting("heavy-reads");
        
        group.MapGet(
            "/get-accounts-user",
            async (ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetAccountsByUserIdQuery(), ct);
                return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemResult();
            }
        )
        .RequireAuthorization()
        .RequireRateLimiting("heavy-reads");

        group.MapPost(
            "/create-account",
            async (CreateAccountCommand command, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(command, ct);
                return result.IsSuccess ? Results.Created() : result.ToProblemResult();
            }
        )
        .RequireAuthorization()
        .RequireRateLimiting("writes");
        
        group.MapPatch(
            "/patch-account/{id:guid}",
            async (Guid id, PatchAccountCommand command, ISender sender, CancellationToken ct) =>
            {
                command = command with { Id = id };
                var result = await sender.Send(command, ct);
                return result.IsSuccess ? Results.Created() : result.ToProblemResult();
            }
        )
        .RequireAuthorization()
        .RequireRateLimiting("writes");
        
        group.MapDelete(
            "/delete-account/{id:guid}",
            async (Guid id, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new DeleteAccountCommand(id), ct);
                return result.IsSuccess ? Results.Created() : result.ToProblemResult();
            }
        )
        .RequireAuthorization()
        .RequireRateLimiting("writes");
        
        group.MapGet(
            "/get-recurring-transaction-details/{id:guid}",
            async (Guid id, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetRecurringTransactionDetailsQuery(id), ct);
                return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemResult();
            }
        )
        .RequireAuthorization()
        .RequireRateLimiting("heavy-reads");
        
        group.MapGet(
            "/get-recurring-transactions-user",
            async (ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetRecurringTransactionsByUserIdQuery(), ct);
                return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemResult();
            }
        )
        .RequireAuthorization()
        .RequireRateLimiting("heavy-reads");
        
        group.MapPost(
            "/create-recurring-transaction",
            async (CreateRecurringTransactionCommand command, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(command, ct);
                return result.IsSuccess ? Results.Created() : result.ToProblemResult();
            }
        )
        .RequireAuthorization()
        .RequireRateLimiting("writes");
        
        group.MapPatch(
            "/patch-recurring-transaction/{id:guid}",
            async (Guid id, PatchRecurringTransactionCommand command, ISender sender, CancellationToken ct) =>
            {
                command = command with { Id = id };
                var result = await sender.Send(command, ct);
                return result.IsSuccess ? Results.Created() : result.ToProblemResult();
            }
        )
        .RequireAuthorization()
        .RequireRateLimiting("writes");
        
        group.MapDelete(
            "/delete-recurring-transaction/{id:guid}",
            async (Guid id, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new DeleteRecurringTransactionCommand(id), ct);
                return result.IsSuccess ? Results.Created() : result.ToProblemResult();
            }
        )
        .RequireAuthorization()
        .RequireRateLimiting("writes");
        
        group.MapGet(
            "/get-transaction-details/{id:guid}",
            async (Guid id, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetTransactionDetailsQuery(id), ct);
                return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemResult();
            }
        )
        .RequireAuthorization()
        .RequireRateLimiting("heavy-reads");
        
        group.MapGet(
            "/get-transactions-user",
            async (ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetTransactionsByUserIdQuery(), ct);
                return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemResult();
            }
        )
        .RequireAuthorization()
        .RequireRateLimiting("heavy-reads");
        
        group.MapGet(
            "/get-transactions-summary",
            async ([FromQuery] DateTime startDate,[FromQuery] DateTime endDate, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetTransactionSummaryQuery(startDate, endDate), ct);
                return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemResult();
            }
        )
        .RequireAuthorization()
        .RequireRateLimiting("heavy-reads");
        
        group.MapPost(
            "/create-transaction",
            async (CreateTransactionCommand command, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(command, ct);
                return result.IsSuccess ? Results.Created() : result.ToProblemResult();
            }
        )
        .RequireAuthorization()
        .RequireRateLimiting("writes");
        
        group.MapPatch(
                "/patch-transaction/{id:guid}",
             async (Guid id, PatchTransactionCommand command, ISender sender, CancellationToken ct) =>
             {
                    command = command with { Id = id };
                    var result = await sender.Send(command, ct);
                    return result.IsSuccess ? Results.Created() : result.ToProblemResult();
             }
        )
        .RequireAuthorization()
        .RequireRateLimiting("writes");
        
        group.MapDelete(
            "/delete-transaction/{id:guid}",
            async (Guid id, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new DeleteTransactionCommand(id), ct);
                return result.IsSuccess ? Results.Created() : result.ToProblemResult();
            }
        )
        .RequireAuthorization()
        .RequireRateLimiting("writes");
        
        return app;
    }
}