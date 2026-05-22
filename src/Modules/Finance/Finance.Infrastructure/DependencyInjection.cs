using Finance.Application.Abstractions.Persistance;
using Finance.Application.Abstractions.Queries;
using Finance.Infrastructure.Persistance.Queries;
using Finance.Infrastructure.Persistance.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Finance.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddFinanceInfrastructure(
        this IServiceCollection services)
    {
        services.AddScoped<IAccountQueries, AccountQueries>();
        services.AddScoped<IRecurringTransactionQueries, RecurringTransactionQueries>();
        services.AddScoped<ITransactionQueries, TransactionQueries>();

        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IRecurringTransactionRepository, RecurringTransactionRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        
        return services;
    }
}