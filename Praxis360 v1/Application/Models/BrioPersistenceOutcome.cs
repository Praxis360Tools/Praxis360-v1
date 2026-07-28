namespace Praxis360_v1.Application.Models;

/// <summary>
/// Statut d'une opération de persistance BRIO atomique.
/// </summary>
public enum BrioPersistenceOutcome
{
    Success = 0,
    ClientNotFound = 1,
    DuplicateExternalReference = 2,
    ValidationFailure = 3,
    PersistenceFailure = 4
}
