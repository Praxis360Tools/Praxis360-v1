using System;

namespace Praxis360_v1.Domain.ValueObjects;

public sealed record ContractNumber
{
    public string Value { get; }

    public const int MaxLength = 100;

    public ContractNumber(string value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        var trimmed = value.Trim();

        if (trimmed.Length == 0)
            throw new ArgumentException("Contract number must not be empty.", nameof(value));

        if (trimmed.Length > MaxLength)
            throw new ArgumentException($"Contract number must not exceed {MaxLength} characters.", nameof(value));

        Value = trimmed;
    }

    public override string ToString() => Value;
}
