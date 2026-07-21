using System;

namespace Praxis360_v1.Domain.ValueObjects;

public sealed record Insurer
{
    public string Key { get; }
    public string DisplayName { get; }

    public Insurer(string key, string displayName)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key must be provided", nameof(key));
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("DisplayName must be provided", nameof(displayName));

        Key = key.Trim();
        DisplayName = displayName.Trim();
    }
}
