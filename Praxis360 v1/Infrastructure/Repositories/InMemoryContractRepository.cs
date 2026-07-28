using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Praxis360.Domain.Types;
using Praxis360_v1.Application.Interfaces;
using Praxis360_v1.Domain.Aggregates;
using Praxis360_v1.Infrastructure.InMemory;

namespace Praxis360_v1.Infrastructure.Repositories;

public sealed class InMemoryContractRepository : IContractRepository
{
    private readonly InMemoryPraxis360Store _store;

    public InMemoryContractRepository(InMemoryPraxis360Store store)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
    }

    // Constructeur sans paramètre pour compatibilité avec les tests existants
    public InMemoryContractRepository()
        : this(new InMemoryPraxis360Store())
    {
    }

    public Task<ContratVie?> GetByIdAsync(Guid id)
    {
        var contract = _store.GetContract(id);
        return Task.FromResult(contract);
    }

    public Task<IReadOnlyCollection<ContratVie>> GetByClientIdAsync(Guid clientId)
    {
        var contracts = _store.GetContractsByClientId(clientId);
        return Task.FromResult<IReadOnlyCollection<ContratVie>>(contracts);
    }

    public Task<ContratVie?> FindByExternalReferenceAsync(Guid clientId, SourceSystem sourceSystem, ReferenceType referenceType, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Reference value must be provided", nameof(value));

        var searchValue = value.Trim();
        var contract = _store.FindContractByExternalReference(
            clientId,
            sourceSystem,
            referenceType,
            searchValue);

        return Task.FromResult(contract);
    }

    public Task SaveAsync(ContratVie contract)
    {
        if (contract is null)
            throw new ArgumentNullException(nameof(contract));

        _store.AddContract(contract);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(ContratVie contract)
    {
        if (contract is null)
            throw new ArgumentNullException(nameof(contract));

        _store.UpdateContract(contract);
        return Task.CompletedTask;
    }
}
