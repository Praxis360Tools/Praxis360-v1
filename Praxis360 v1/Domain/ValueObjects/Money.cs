using System;
using Praxis360.Domain.Types;

namespace Praxis360_v1.Domain.ValueObjects;

public sealed record Money
{
    public decimal Amount { get; }
    public Currency Currency { get; }

    public Money(decimal amount, Currency currency)
    {
        if (!Enum.IsDefined(typeof(Currency), currency))
            throw new ArgumentException("Currency must be a defined value.", nameof(currency));

        Amount = amount;
        Currency = currency;
    }

    public Money Add(Money other)
    {
        if (other is null) throw new ArgumentNullException(nameof(other));
        if (!Equals(Currency, other.Currency)) throw new InvalidOperationException("Cannot add amounts with different currencies.");
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (other is null) throw new ArgumentNullException(nameof(other));
        if (!Equals(Currency, other.Currency)) throw new InvalidOperationException("Cannot subtract amounts with different currencies.");
        return new Money(Amount - other.Amount, Currency);
    }

    public override string ToString() => $"{Amount} {Currency}";
}
