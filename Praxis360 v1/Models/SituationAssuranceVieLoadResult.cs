using System;

namespace Praxis360_v1.Models;

public sealed class SituationAssuranceVieLoadResult
{
    public SituationAssuranceVieReadModel? Situation { get; init; }
    public SituationAssuranceVieLoadStatus Status { get; init; }

    public SituationAssuranceVieLoadResult(SituationAssuranceVieReadModel? situation, SituationAssuranceVieLoadStatus status)
    {
        Situation = situation;
        Status = status;
    }
}

public enum SituationAssuranceVieLoadStatus
{
    ClientLoaded,
    NoClientsAvailable,
    MultipleClientsRequireSelection,
    ClientNotFound
}
