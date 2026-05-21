using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Host.ProblemDetails;

public sealed class ValidationExceptionHandler
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ValidationException validationException)
        {
            return false;
        }

        var errors = validationException.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ErrorMessage).ToArray()
            );

        var problemDetails = new ValidationProblemDetails(errors)
        {
            Title = "Validation Error",
            Status = StatusCodes.Status400BadRequest,
            Type = "about:blank"
        };

        problemDetails.Extensions["traceId"] =
            httpContext.TraceIdentifier;

        httpContext.Response.StatusCode =
            StatusCodes.Status400BadRequest;

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);

        return true;
    }
}