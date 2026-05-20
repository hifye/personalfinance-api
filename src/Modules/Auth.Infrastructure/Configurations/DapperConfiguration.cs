using Auth.Infrastructure.Configurations.TypeHandlers;
using Dapper;
using SharedKernel.TypeHandlers;

namespace Auth.Infrastructure.Configurations;

public static class DapperConfiguration
{
    public static void RegisterTypeHandlers()
    {
        SqlMapper.AddTypeHandler(new EmailTypeHandler());
        SqlMapper.AddTypeHandler(new PriceTypeHandler());
    }
}