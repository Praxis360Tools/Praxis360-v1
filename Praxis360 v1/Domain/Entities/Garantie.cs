using System;
using Praxis360_v1.Domain.ValueObjects;
using Praxis360_v1.Domain.Types;

namespace Praxis360_v1.Domain.Entities;

public sealed class Garantie
{
    public Guid Id { get; }

    // Type of garantie is mandatory per architecture decision
    public TypeGarantie GarantieType { get; }

    // Optional label when needed for explanation
    public string? Label { get; }

    // Monetary coverage may be absent in incomplete dossiers
    public Money? Coverage { get; }

    // Period may be absent for incomplete dossiers
    public DateRange? Period { get; }

    public Garantie(
        Guid id,
        TypeGarantie garantieType,
        string? label = null,
        Money? coverage = null,
        DateRange? period = null)
    {
        Id = id == Guid.Empty ? throw new ArgumentException("Id must be provided.", nameof(id)) : id;

        GarantieType = garantieType;

        Label = string.IsNullOrWhiteSpace(label) ? null : label.Trim();

        Coverage = coverage;

        Period = period;
    }
}
