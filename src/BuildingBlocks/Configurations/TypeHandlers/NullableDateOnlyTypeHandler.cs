using System.Data;
using Dapper;

namespace BuildingBlocks.Configurations.TypeHandlers;

public sealed class NullableDateOnlyTypeHandler
    : SqlMapper.TypeHandler<DateOnly?>
{
    public override void SetValue(IDbDataParameter parameter, DateOnly? value)
    {
        parameter.Value = value;
    }

    public override DateOnly? Parse(object value)
    {
        if (value is null || value == DBNull.Value)
            return null;

        return value switch
        {
            DateOnly dateOnly => dateOnly,
            DateTime dateTime => DateOnly.FromDateTime(dateTime),
            _ => throw new DataException(
                $"Cannot convert {value.GetType()} to DateOnly?")
        };
    }
}