using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Common;

namespace Auth.Application.Behaviors;

/// <summary>
/// Middleware do MediatR para logar informações sobre a execução de requisições, incluindo payload, tempo de execução e resultados.
/// </summary>
/// <typeparam name="TRequest">Tipo da requisição.</typeparam>
/// <typeparam name="TResponse">Tipo da resposta.</typeparam>
public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var requestId = Guid.NewGuid().ToString()[..8]; // identificador curto para correlacionar logs

        logger.LogInformation(
            "[{RequestId}] Handling {RequestName} | Payload: {@Request}",
            requestId, requestName, request
        );

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await next();
            stopwatch.Stop();

            // se o TResponse é um Result, loga sucesso ou falha
            if (response is Result result)
            {
                if (result.IsSuccess)
                {
                    logger.LogInformation(
                        "[{RequestId}] {RequestName} completed successfully in {ElapsedMs}ms",
                        requestId, requestName, stopwatch.ElapsedMilliseconds
                    );
                }
                else
                {
                    logger.LogWarning(
                        "[{RequestId}] {RequestName} failed in {ElapsedMs}ms | Error: {Error} | ErrorType: {ErrorType}",
                        requestId, requestName, stopwatch.ElapsedMilliseconds, result.Error, result.ErrorType
                    );
                }
            }
            else
            {
                logger.LogInformation(
                    "[{RequestId}] {RequestName} completed in {ElapsedMs}ms",
                    requestId, requestName, stopwatch.ElapsedMilliseconds
                );
            }

            // aviso se demorar mais de 500ms
            if (stopwatch.ElapsedMilliseconds > 500)
            {
                logger.LogWarning(
                    "[{RequestId}] {RequestName} is running slow ({ElapsedMs}ms). Consider optimizing.",
                    requestId, requestName, stopwatch.ElapsedMilliseconds
                );
            }

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            logger.LogError(
                ex,
                "[{RequestId}] {RequestName} threw an unhandled exception in {ElapsedMs}ms | Message: {Message}",
                requestId, requestName, stopwatch.ElapsedMilliseconds, ex.Message
            );

            throw;
        }
    }
}