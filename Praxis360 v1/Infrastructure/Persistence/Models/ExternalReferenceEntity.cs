using System;
using Praxis360.Domain.Types;

namespace Praxis360_v1.Infrastructure.Persistence.Models;

public sealed class ExternalReferenceEntity
{
    public int Id { get; set; }
    public Guid ContractId { get; set; }
    public Guid ClientId { get; set; }
    public SourceSystem SourceSystem { get; set; }
    public ReferenceType ReferenceType { get; set; }
    public string Value { get; set; } = string.Empty;

    public ContractEntity Contract { get; set; } = null!;
}
