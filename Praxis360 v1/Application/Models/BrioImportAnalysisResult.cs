using System.Collections.Generic;
using System.Linq;

namespace Praxis360_v1.Application.Models;

public sealed class BrioImportAnalysisResult
{
    public IReadOnlyList<BrioAnalyzedLine> AnalyzedLines { get; }
    public IReadOnlyDictionary<string, BrioClientCandidate> ClientCandidates { get; }
    public IReadOnlyList<BrioContractCandidate> ContractCandidates { get; }
    public IReadOnlyList<ImportAnalysisIssue> AllIssues { get; }

    public BrioImportAnalysisResult(
        IReadOnlyList<BrioAnalyzedLine> analyzedLines,
        IReadOnlyDictionary<string, BrioClientCandidate> clientCandidates,
        IReadOnlyList<BrioContractCandidate> contractCandidates,
        IReadOnlyList<ImportAnalysisIssue> structuralErrors)
    {
        AnalyzedLines = analyzedLines ?? throw new System.ArgumentNullException(nameof(analyzedLines));
        ClientCandidates = clientCandidates ?? throw new System.ArgumentNullException(nameof(clientCandidates));
        ContractCandidates = contractCandidates ?? throw new System.ArgumentNullException(nameof(contractCandidates));

        var allIssues = new List<ImportAnalysisIssue>();

        allIssues.AddRange(structuralErrors ?? throw new System.ArgumentNullException(nameof(structuralErrors)));

        foreach (var line in analyzedLines)
        {
            allIssues.AddRange(line.Issues);
        }
        AllIssues = allIssues;
    }

    public int TotalLinesAnalyzed => AnalyzedLines.Count;
    public int LinesWithBlockingErrors => AnalyzedLines.Count(l => l.HasBlockingErrors);
    public int LinesWithWarnings => AnalyzedLines.Count(l => l.Issues.Any(i => i.Severity == ImportIssueSeverity.Warning));
    public int BlockingErrorCount => AllIssues.Count(i => i.Severity == ImportIssueSeverity.BlockingError);
    public int WarningCount => AllIssues.Count(i => i.Severity == ImportIssueSeverity.Warning);
    public bool HasBlockingErrors => BlockingErrorCount > 0;
    public bool CanProceed => !HasBlockingErrors && ContractCandidates.Count > 0;
}
