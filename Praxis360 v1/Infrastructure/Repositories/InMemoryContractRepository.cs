using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Praxis360.Domain.Types;
using Praxis360_v1.Application.Interfaces;
using Praxis360_v1.Domain.Aggregates;

namespace Praxis360_v1.Infrastructure.Repositories;

public sealed class InMemoryContractRepository : IContractRepository
{
    private readonly Dictionary<Guid, ContratVie> _contracts = new();
    private readonly object _lock = new();

    public Task<ContratVie?> GetByIdAsync(Guid id)
    {
        lock (_lock)
        {
            _contracts.TryGetValue(id, out var contract);
            return Task.FromResult(contract);
        }
    }

    public Task<IReadOnlyCollection<ContratVie>> GetByClientIdAsync(Guid clientId)
    {
        lock (_lock)
        {
            var contracts = _contracts.Values
                .Where(c => c.ClientId == clientId)
                .ToList();
            return Task.FromResult<IReadOnlyCollection<ContratVie>>(contracts);
        }
    }

    public Task<ContratVie?> FindByExternalReferenceAsync(Guid clientId, SourceSystem sourceSystem, ReferenceType referenceType, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Reference value must be provided", nameof(value));

        lock (_lock)
        {
            var searchValue = value.Trim();

            var contract = _contracts.Values
                .Where(c => c.ClientId == clientId)
                .FirstOrDefault(c => c.ExternalReferences.Any(r =>
                    r.SourceSystem == sourceSystem &&
                    r.ReferenceType == referenceType &&
                    r.Value.Equals(searchValue, StringComparison.OrdinalIgnoreCase)));

            return Task.FromResult(contract);
        }
    }

    public Task SaveAsync(ContratVie contract)
    {
        if (contract is null)
            throw new ArgumentNullException(nameof(contract));

        lock (_lock)
        {
            if (_contracts.ContainsKey(contract.Id))
                throw new InvalidOperationException($"Contract with Id {contract.Id} already exists. Use UpdateAsync to modify existing contracts.");

            _contracts[contract.Id] = contract;
        }

        return Task.CompletedTask;
    }

    public Task UpdateAsync(ContratVie contract)
    {
        if (contract is null)
            throw new ArgumentNullException(nameof(contract));

        lock (_lock)
        {
            if (!_contracts.ContainsKey(contract.Id))
                throw new InvalidOperationException($"Contract with Id {contract.Id} does not exist. Use SaveAsync to add new contracts.");

            _contracts[contract.Id] = contract;
        }

        return Task.CompletedTask;
    }
}
