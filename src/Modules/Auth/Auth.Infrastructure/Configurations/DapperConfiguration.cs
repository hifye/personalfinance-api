using Auth.Infrastructure.Configurations.TypeHandlers;
using Dapper;

namespace Auth.Infrastructure.Configurations;

public static class DapperConfiguration
{
    public static void RegisterTypeHandlers()
    {
        SqlMapper.AddTypeHandler(new EmailTypeHandler());
        SqlMapper.AddTypeHandler(new PriceTypeHandler());
    }
}