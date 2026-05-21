using BuildingBlocks.Configurations.TypeHandlers;
using Dapper;

namespace BuildingBlocks.Configurations;

public static class DapperConfiguration
{
    public static void RegisterTypeHandlers()
    {
        SqlMapper.AddTypeHandler(new EmailTypeHandler());
        SqlMapper.AddTypeHandler(new PriceTypeHandler());
    }
}