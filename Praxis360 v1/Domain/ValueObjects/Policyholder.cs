using System;
using Praxis360.Domain.Types;

namespace Praxis360_v1.Domain.ValueObjects;

public sealed record Policyholder
{
    public PolicyholderType Type { get; }
    public string Name { get; }

    public Policyholder(PolicyholderType type, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name must be provided", nameof(name));

        Type = type;
        Name = name.Trim();
    }
}
