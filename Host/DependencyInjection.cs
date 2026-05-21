using Host.ProblemDetails;

namespace Host;

public static class DependencyInjection
{
    public static IServiceCollection AddHost(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddProblemDetails();
        
        services.AddExceptionHandler<ValidationExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        
        
        return services;
    }
}