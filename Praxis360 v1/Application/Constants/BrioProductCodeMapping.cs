using Praxis360.Domain.Types;

namespace Praxis360_v1.Application.Constants;

public static class BrioProductCodeMapping
{
    public static ContractType? MapProductCode(string? productCode)
    {
        if (string.IsNullOrWhiteSpace(productCode))
            return null;

        var normalized = productCode.Trim().ToUpperInvariant();

        return normalized switch
        {
            "FSPS" => ContractType.PLCI,
            "ESPSI" => ContractType.PLCI,
            "EIP" => ContractType.EIP,
            _ => null
        };
    }

    public static bool IsKnownProductCode(string? productCode)
    {
        if (string.IsNullOrWhiteSpace(productCode))
            return false;

        var normalized = productCode.Trim().ToUpperInvariant();

        return normalized is "FSPS" or "ESPSI" or "EIP";
    }
}
