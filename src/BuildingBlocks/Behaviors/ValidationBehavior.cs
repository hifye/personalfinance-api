using FluentValidation;
using MediatR;

namespace BuildingBlocks.Behaviors;

/// <summary>
/// Behavior do pipeline do MediatR responsável por executar a validação de requisições usando o FluentValidation.
/// </summary>
/// <typeparam name="TRequest">Tipo da requisição que deve implementar <see cref="IRequest{TResponse}"/>.</typeparam>
/// <typeparam name="TResponse">Tipo da resposta retornada pela requisição.</typeparam>
/// <param name="validators">Coleção de validadores registrados para o tipo <typeparamref name="TRequest"/>.</param>
public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var results = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failures  = results
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();
            if (failures.Count != 0)
                throw new ValidationException(failures);
        }
        return await next(cancellationToken);
    }
}