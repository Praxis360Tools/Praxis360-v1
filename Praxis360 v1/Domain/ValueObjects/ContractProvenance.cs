using System;
using Praxis360.Domain.Types;

namespace Praxis360_v1.Domain.ValueObjects;

public sealed record ContractProvenance
{
    public SourceSystem SourceSystem { get; }
    public string? RawInsurerName { get; }
    public DateTime ImportedAtUtc { get; }
    public DateOnly? SourceSnapshotDate { get; }

    public ContractProvenance(
        SourceSystem sourceSystem,
        DateTime importedAtUtc,
        string? rawInsurerName = null,
        DateOnly? sourceSnapshotDate = null)
    {
        if (importedAtUtc.Kind != DateTimeKind.Utc)
            throw new ArgumentException("ImportedAtUtc must be in UTC (DateTimeKind.Utc).", nameof(importedAtUtc));

        SourceSystem = sourceSystem;
        ImportedAtUtc = importedAtUtc;
        RawInsurerName = string.IsNullOrWhiteSpace(rawInsurerName) ? null : rawInsurerName.Trim();
        SourceSnapshotDate = sourceSnapshotDate;
    }
}
