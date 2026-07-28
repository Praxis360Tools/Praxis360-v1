namespace Praxis360_v1.Application.Models;

/// <summary>
/// Problème structuré identifié lors d'une opération de persistance BRIO.
/// </summary>
public sealed class BrioPersistenceIssue
{
    public BrioPersistenceSeverity Severity { get; init; }
    public string Message { get; init; } = string.Empty;
    public Guid? AffectedContractId { get; init; }
}
