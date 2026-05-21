using Microsoft.AspNetCore.Diagnostics;

namespace Host.ProblemDetails;

public sealed class GlobalExceptionHandler
    : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(
            exception,
            "Unhandled exception occurred. TraceId: {TraceId}",
            httpContext.TraceIdentifier);

        var problemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Title = "Server Error",
            Status = StatusCodes.Status500InternalServerError,
            Type = "about:blank",
            Detail = "An unexpected error occurred."
        };

        problemDetails.Extensions["traceId"] =
            httpContext.TraceIdentifier;

        httpContext.Response.StatusCode =
            StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);

        return true;
    }
}