using System;

namespace Praxis360_v1.Domain.ValueObjects;

public sealed record Percentage
{
    private readonly decimal _fraction;

    private Percentage(decimal fraction)
    {
        _fraction = fraction;
    }

    public decimal Fraction => _fraction;

    public static Percentage FromFraction(decimal value)
    {
        if (value < 0m || value > 1m) throw new ArgumentOutOfRangeException(nameof(value), "Fraction must be between 0 and 1 inclusive.");
        return new Percentage(value);
    }

    public static Percentage FromPercent(decimal percent)
    {
        var fraction = percent / 100m;
        return FromFraction(fraction);
    }

    public override string ToString() => _fraction.ToString();
}
