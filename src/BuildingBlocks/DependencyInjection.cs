using System.Reflection;
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Behaviors;
using BuildingBlocks.Identity;
using BuildingBlocks.Infrastructure.Persistance;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks;

public static class DependencyInjection
{
    public static IServiceCollection AddBuildingBlocks(this IServiceCollection services, params Assembly[] moduleAssemblies)
    {
        // Persistência
        services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Identidade
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();

        // MediatR com behaviors — registra os assemblies dos módulos
        services.AddMediatR(opt =>
        {
            opt.RegisterServicesFromAssemblies(moduleAssemblies);
            opt.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            opt.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        // Validators dos módulos
        foreach (var assembly in moduleAssemblies)
            services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}