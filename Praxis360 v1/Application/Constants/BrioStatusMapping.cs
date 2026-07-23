using Praxis360.Domain.Types;

namespace Praxis360_v1.Application.Constants;

public static class BrioStatusMapping
{
    private const string KnownStatusActive = "En cours";

    public static ContractStatus? MapStatus(string? rawStatus)
    {
        if (string.IsNullOrWhiteSpace(rawStatus))
            return null;

        var trimmed = rawStatus.Trim();

        if (trimmed.Equals(KnownStatusActive, StringComparison.Ordinal))
            return ContractStatus.Active;

        return null;
    }

    public static bool IsKnownStatus(string? rawStatus)
    {
        if (string.IsNullOrWhiteSpace(rawStatus))
            return false;

        var trimmed = rawStatus.Trim();

        return trimmed.Equals(KnownStatusActive, StringComparison.Ordinal);
    }
}
