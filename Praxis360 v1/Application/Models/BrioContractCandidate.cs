using System.Collections.Generic;
using Praxis360.Domain.Types;

namespace Praxis360_v1.Application.Models;

public sealed class BrioContractCandidate
{
    public string NormalizedPolicyNumber { get; }
    public string ClientNormalizedIdentity { get; }
    public ContractType? MappedType { get; }
    public string? RawProductCode { get; }
    public string? RawProductLabel { get; }
    public string? RawInsurerName { get; }
    public string? RawStatus { get; }
    public List<int> SourceLineNumbers { get; }

    public BrioContractCandidate(
        string normalizedPolicyNumber,
        string clientNormalizedIdentity,
        ContractType? mappedType,
        string? rawProductCode,
        string? rawProductLabel,
        string? rawInsurerName,
        string? rawStatus,
        int sourceLineNumber)
    {
        if (string.IsNullOrWhiteSpace(normalizedPolicyNumber))
            throw new System.ArgumentException("NormalizedPolicyNumber must be provided", nameof(normalizedPolicyNumber));
        if (string.IsNullOrWhiteSpace(clientNormalizedIdentity))
            throw new System.ArgumentException("ClientNormalizedIdentity must be provided", nameof(clientNormalizedIdentity));

        NormalizedPolicyNumber = normalizedPolicyNumber.Trim().ToUpperInvariant();
        ClientNormalizedIdentity = clientNormalizedIdentity;
        MappedType = mappedType;
        RawProductCode = string.IsNullOrWhiteSpace(rawProductCode) ? null : rawProductCode.Trim();
        RawProductLabel = string.IsNullOrWhiteSpace(rawProductLabel) ? null : rawProductLabel.Trim();
        RawInsurerName = string.IsNullOrWhiteSpace(rawInsurerName) ? null : rawInsurerName.Trim();
        RawStatus = string.IsNullOrWhiteSpace(rawStatus) ? null : rawStatus.Trim();
        SourceLineNumbers = new List<int> { sourceLineNumber };
    }

    public void AddSourceLine(int lineNumber)
    {
        if (!SourceLineNumbers.Contains(lineNumber))
            SourceLineNumbers.Add(lineNumber);
    }
}
