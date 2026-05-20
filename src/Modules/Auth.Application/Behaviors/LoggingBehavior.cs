using MediatR;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Collections;

namespace Auth.Application.Behaviors;

/// <summary>
/// Behavior do MediatR responsável pelo log das requisições no módulo de autenticação.
/// Implementa o mascaramento de dados sensíveis para evitar exposição em logs.
/// </summary>
/// <typeparam name="TRequest">Tipo da requisição.</typeparam>
/// <typeparam name="TResponse">Tipo da resposta.</typeparam>
/// <param name="logger">Logger injetado.</param>
public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private static readonly string[] SensitivePropertyNames = ["Password", "ConfirmPassword", "Token", "RefreshToken", "Secret", "Key", "ApiKey", "PasswordHash"];

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        logger.LogInformation("Iniciando requisição {RequestName}", requestName);

        try
        {
            var maskedRequest = MaskObject(request);
            logger.LogInformation("Dados da requisição {RequestName}: {@RequestData}", requestName, maskedRequest);

            var response = await next();

            logger.LogInformation("Requisição {RequestName} concluída com sucesso", requestName);
            
            var maskedResponse = MaskObject(response);
            logger.LogInformation("Dados de retorno da requisição {RequestName}: {@ResponseData}", requestName, maskedResponse);

            return response;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Requisição {RequestName} falhou", requestName);
            throw;
        }
    }

    private object? MaskObject(object? obj)
    {
        if (obj == null) return null;

        var type = obj.GetType();

        // Tipos simples: retorna o valor original
        if (type.IsPrimitive || type.IsEnum || obj is string || obj is decimal || obj is DateTime || obj is Guid)
        {
            return obj;
        }

        // Coleções (exceto strings)
        if (obj is IEnumerable enumerable)
        {
            var items = new List<object?>();
            foreach (var item in enumerable)
            {
                items.Add(MaskObject(item));
            }
            return items;
        }

        // Objetos complexos
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var maskedData = new Dictionary<string, object?>();

        foreach (var prop in properties)
        {
            try
            {
                var value = prop.GetValue(obj);

                if (SensitivePropertyNames.Any(s => prop.Name.Contains(s, StringComparison.OrdinalIgnoreCase)))
                {
                    maskedData.Add(prop.Name, "***");
                }
                else
                {
                    maskedData.Add(prop.Name, MaskObject(value));
                }
            }
            catch
            {
                maskedData.Add(prop.Name, "[Erro ao ler propriedade]");
            }
        }

        return maskedData;
    }
}
