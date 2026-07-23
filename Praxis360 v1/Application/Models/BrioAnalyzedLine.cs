using System.Collections.Generic;
using Praxis360.Domain.Types;

namespace Praxis360_v1.Application.Models;

public sealed class BrioAnalyzedLine
{
    public int LineNumber { get; }
    public string? ClientNormalizedIdentity { get; }
    public string? NormalizedPolicyNumber { get; }
    public string? RawProductCode { get; }
    public string? RawProductLabel { get; }
    public ContractType? MappedContractType { get; }
    public List<ImportAnalysisIssue> Issues { get; }

    public BrioAnalyzedLine(
        int lineNumber,
        string? clientNormalizedIdentity,
        string? normalizedPolicyNumber,
        string? rawProductCode,
        string? rawProductLabel,
        ContractType? mappedContractType)
    {
        LineNumber = lineNumber;
        ClientNormalizedIdentity = clientNormalizedIdentity;
        NormalizedPolicyNumber = normalizedPolicyNumber;
        RawProductCode = rawProductCode;
        RawProductLabel = rawProductLabel;
        MappedContractType = mappedContractType;
        Issues = new List<ImportAnalysisIssue>();
    }

    public void AddIssue(string code, ImportIssueSeverity severity, string message, string? fieldName = null)
    {
        Issues.Add(new ImportAnalysisIssue(code, severity, message, LineNumber, fieldName));
    }

    public bool HasBlockingErrors => Issues.Exists(i => i.Severity == ImportIssueSeverity.BlockingError);
}
