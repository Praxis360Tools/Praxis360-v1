using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Praxis360.Domain.Types;
using Praxis360_v1.Domain.Aggregates;

namespace Praxis360_v1.Application.Interfaces;

public interface IContractRepository
{
    Task<ContratVie?> GetByIdAsync(Guid id);
    Task<IReadOnlyCollection<ContratVie>> GetByClientIdAsync(Guid clientId);
    Task<ContratVie?> FindByExternalReferenceAsync(Guid clientId, SourceSystem sourceSystem, ReferenceType referenceType, string value);
    Task SaveAsync(ContratVie contract);
    Task UpdateAsync(ContratVie contract);
}
