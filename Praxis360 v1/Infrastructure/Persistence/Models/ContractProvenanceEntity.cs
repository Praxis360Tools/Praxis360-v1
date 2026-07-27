using System;
using Praxis360.Domain.Types;

namespace Praxis360_v1.Infrastructure.Persistence.Models;

public sealed class ContractProvenanceEntity
{
    public int Id { get; set; }
    public Guid ContractId { get; set; }
    public SourceSystem SourceSystem { get; set; }
    public string? RawInsurerName { get; set; }
    public DateTime ImportedAtUtc { get; set; }
    public DateOnly? SourceSnapshotDate { get; set; }

    public ContractEntity Contract { get; set; } = null!;
}
