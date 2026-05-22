using SharedKernel.Common;

namespace SharedKernel.ValueObjects;

public sealed record Price
{
    public decimal Value { get; }
    
    private Price(decimal value) => Value = value;

    public static Result<Price> Create(decimal value)
    {
        return Guard
            .AgainstOutOfRange(value < 1, "Price cannot be negative.")
            .Map(() => new Price(value));
    }
}