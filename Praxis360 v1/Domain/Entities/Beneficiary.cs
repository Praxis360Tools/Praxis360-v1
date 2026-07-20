using System;
using Praxis360.Domain.Types;
using Praxis360_v1.Domain.ValueObjects;

namespace Praxis360_v1.Domain.Entities;

public sealed class Beneficiary
{
    public Guid Id { get; }

    // Name is optional because beneficiary may be an organisation or unspecified in partial dossiers
    public string? Name { get; private set; }

    public BeneficiaryType Type { get; }

    public Percentage Share { get; private set; }

    public Beneficiary(Guid id, string? name, BeneficiaryType type, Percentage share)
    {
        Id = id == Guid.Empty ? throw new ArgumentException("Id must be provided.", nameof(id)) : id;

        Name = string.IsNullOrWhiteSpace(name) ? null : name.Trim();

        Type = type;

        Share = share ?? throw new ArgumentNullException(nameof(share));
    }

    internal void UpdateShareInternal(Percentage newShare)
    {
        Share = newShare ?? throw new ArgumentNullException(nameof(newShare));
    }
}
