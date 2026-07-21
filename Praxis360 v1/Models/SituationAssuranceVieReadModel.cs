using System;
using System.Collections.Generic;
using Praxis360.Domain.Types;
using Praxis360_v1.Domain.ValueObjects;

namespace Praxis360_v1.Models;

public sealed record SituationAssuranceVieContractReadModel(
    string Number,
    ContractType ContractType,
    string ContractTypeLabel,
    string? InsurerKey,
    string InsurerDisplayName,
    ContractStatus Status,
    string StatusLabel,
    string StatusCssClass,
    IReadOnlyList<string> GuaranteeLabels,
    string Description
);

public sealed class SituationAssuranceVieReadModel
{
    public Guid ClientId { get; init; }

    public string ClientDisplayName { get; init; } = string.Empty;

    public int TotalContracts { get; init; }

    public int CurrentContracts { get; init; }

    public Money? ReserveAcquise { get; init; }

    public Money? CapitalATerme { get; init; }

    public Money? CapitalDeces { get; init; }

    public Money? RevenuGaranti { get; init; }

    // Minimal typed origin information: list of contracts with number and status
    public IReadOnlyList<SituationAssuranceVieContractReadModel> Contracts { get; init; } = Array.Empty<SituationAssuranceVieContractReadModel>();
}
