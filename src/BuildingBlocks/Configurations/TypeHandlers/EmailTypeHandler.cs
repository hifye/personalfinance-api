using System.Data;
using Dapper;
using SharedKernel.ValueObjects;

namespace BuildingBlocks.Configurations.TypeHandlers;

/// <summary>
/// Manipulador de tipo para mapear o objeto de valor <see cref="Email"/> em consultas do Dapper.
/// </summary>
public sealed class EmailTypeHandler : SqlMapper.TypeHandler<Email>
{
    public override void SetValue(IDbDataParameter parameter, Email? value)
        => parameter.Value = value?.Address;
    
    public override Email? Parse(object value)
    {
        var result = Email.Create(value.ToString()!);
        if (result.IsFailure)
            throw new DataException($"Invalid email from database: {value}.");
        return result.Value;
    }
}