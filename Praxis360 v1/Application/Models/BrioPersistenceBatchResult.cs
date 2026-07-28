namespace Praxis360_v1.Application.Models;

/// <summary>
/// Résultat d'une opération atomique de persistance BRIO.
/// </summary>
public sealed class BrioPersistenceBatchResult
{
    public Guid? ClientId { get; init; }
    public bool ClientWasPersisted { get; init; }
    public IReadOnlyList<Guid> PersistedContractIds { get; init; } = Array.Empty<Guid>();
    public IReadOnlyList<BrioPersistenceIssue> Issues { get; init; } = Array.Empty<BrioPersistenceIssue>();
    public BrioPersistenceOutcome Outcome { get; init; }

    public static BrioPersistenceBatchResult Success(
        Guid clientId,
        bool clientWasPersisted,
        IReadOnlyList<Guid> persistedContractIds)
    {
        return new BrioPersistenceBatchResult
        {
            ClientId = clientId,
            ClientWasPersisted = clientWasPersisted,
            PersistedContractIds = persistedContractIds,
            Outcome = BrioPersistenceOutcome.Success
        };
    }

    public static BrioPersistenceBatchResult ClientNotFound(Guid clientId)
    {
        return new BrioPersistenceBatchResult
        {
            ClientId = clientId,
            Outcome = BrioPersistenceOutcome.ClientNotFound,
            Issues = new[]
            {
                new BrioPersistenceIssue
                {
                    Severity = BrioPersistenceSeverity.Error,
                    Message = "Le client spécifié n'existe pas."
                }
            }
        };
    }

    public static BrioPersistenceBatchResult ValidationFailure(
        IReadOnlyList<BrioPersistenceIssue> issues,
        Guid? clientId = null)
    {
        return new BrioPersistenceBatchResult
        {
            ClientId = clientId,
            Outcome = BrioPersistenceOutcome.ValidationFailure,
            Issues = issues
        };
    }

    public static BrioPersistenceBatchResult DuplicateExternalReference(
        Guid? affectedContractId,
        string message,
        Guid? clientId = null)
    {
        return new BrioPersistenceBatchResult
        {
            ClientId = clientId,
            Outcome = BrioPersistenceOutcome.DuplicateExternalReference,
            Issues = new[]
            {
                new BrioPersistenceIssue
                {
                    Severity = BrioPersistenceSeverity.Error,
                    Message = message,
                    AffectedContractId = affectedContractId
                }
            }
        };
    }

    public static BrioPersistenceBatchResult PersistenceFailure(string message, Guid? clientId = null)
    {
        return new BrioPersistenceBatchResult
        {
            ClientId = clientId,
            Outcome = BrioPersistenceOutcome.PersistenceFailure,
            Issues = new[]
            {
                new BrioPersistenceIssue
                {
                    Severity = BrioPersistenceSeverity.Error,
                    Message = message
                }
            }
        };
    }
}
