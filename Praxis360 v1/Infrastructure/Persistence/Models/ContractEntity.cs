using System;
using System.Collections.Generic;
using Praxis360.Domain.Types;

namespace Praxis360_v1.Infrastructure.Persistence.Models;

public sealed class ContractEntity
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public ContractType Type { get; set; }
    public ContractStatus Status { get; set; }
    public Guid ClientId { get; set; }

    public ClientEntity Client { get; set; } = null!;
    public ICollection<ExternalReferenceEntity> ExternalReferences { get; set; } = new List<ExternalReferenceEntity>();
    public ICollection<ContractProvenanceEntity> ContractProvenances { get; set; } = new List<ContractProvenanceEntity>();
}
