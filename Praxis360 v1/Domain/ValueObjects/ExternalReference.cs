using System;
using Praxis360.Domain.Types;

namespace Praxis360_v1.Domain.ValueObjects;

public sealed record ExternalReference
{
    public SourceSystem SourceSystem { get; }
    public ReferenceType ReferenceType { get; }
    public string Value { get; }

    public ExternalReference(SourceSystem sourceSystem, ReferenceType referenceType, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value must be provided", nameof(value));

        SourceSystem = sourceSystem;
        ReferenceType = referenceType;
        Value = value.Trim();
    }
}
