using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Middlewares;

public class CorrelationMiddleware(RequestDelegate next)
{
    public const string CorrelationIdHeader = "X-Correlation-Id";

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers.TryGetValue(CorrelationIdHeader, out var existingId)
            ? existingId.ToString()
            : Guid.NewGuid().ToString();
        
        context.TraceIdentifier = correlationId;
        
        context.Response.Headers[CorrelationIdHeader] = correlationId;
        
        using(Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId))
        {
            await next(context);
        }
    }
}