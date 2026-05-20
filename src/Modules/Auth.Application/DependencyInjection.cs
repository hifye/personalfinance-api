using Auth.Application.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthApplication(this IServiceCollection services)
    {
        services.AddMediatR(opt =>
        {
            opt.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            opt.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            opt.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });
        
        return services;
    }
}