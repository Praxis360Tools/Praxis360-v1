using Praxis360.Domain.Types;
using Praxis360_v1.Domain.Aggregates;
using Praxis360_v1.Domain.Entities;

namespace Praxis360_v1.Infrastructure.InMemory;

/// <summary>
/// Store en mémoire partagé pour les clients, contrats et références BRIO.
/// Permet aux repositories InMemory et au service de persistance de partager le même état.
/// </summary>
public sealed class InMemoryPraxis360Store
{
    private readonly object _lock = new();
    private readonly Dictionary<Guid, Client> _clients = new();
    private readonly Dictionary<Guid, ContratVie> _contracts = new();

    // Index pour recherche rapide par référence externe : (ClientId, SourceSystem, ReferenceType, Value) -> ContractId
    private readonly Dictionary<(Guid ClientId, SourceSystem SourceSystem, ReferenceType ReferenceType, string Value), Guid> _externalReferenceIndex = new();

    public void AddClient(Client client)
    {
        lock (_lock)
        {
            if (_clients.ContainsKey(client.Id))
            {
                throw new InvalidOperationException($"Client {client.Id} already exists.");
            }
            _clients[client.Id] = client;
        }
    }

    public void UpdateClient(Client client)
    {
        lock (_lock)
        {
            if (!_clients.ContainsKey(client.Id))
            {
                throw new InvalidOperationException($"Client {client.Id} not found.");
            }
            _clients[client.Id] = client;
        }
    }

    public Client? GetClient(Guid clientId)
    {
        lock (_lock)
        {
            return _clients.GetValueOrDefault(clientId);
        }
    }

    public IReadOnlyList<Client> GetAllClients()
    {
        lock (_lock)
        {
            return _clients.Values.ToList();
        }
    }

    public IReadOnlyList<Client> SearchClientsByIdentity(string normalizedIdentity)
    {
        // Recherche non implémentée dans le store car Client n'a pas de NormalizedIdentity
        // Le repository implémentent la recherche par prénom/nom/date
        throw new NotSupportedException("Use repository SearchByIdentityAsync instead.");
    }

    public void AddContract(ContratVie contract)
    {
        lock (_lock)
        {
            if (_contracts.ContainsKey(contract.Id))
            {
                throw new InvalidOperationException($"Contract {contract.Id} already exists.");
            }

            // Vérifier les doublons de références externes
            foreach (var reference in contract.ExternalReferences)
            {
                var key = (contract.ClientId, reference.SourceSystem, reference.ReferenceType, reference.Value);
                if (_externalReferenceIndex.ContainsKey(key))
                {
                    throw new InvalidOperationException(
                        $"External reference ({reference.SourceSystem}, {reference.ReferenceType}, {reference.Value}) already exists for ClientId {contract.ClientId}.");
                }
            }

            _contracts[contract.Id] = contract;

            // Indexer les références externes
            foreach (var reference in contract.ExternalReferences)
            {
                var key = (contract.ClientId, reference.SourceSystem, reference.ReferenceType, reference.Value);
                _externalReferenceIndex[key] = contract.Id;
            }
        }
    }

    public void UpdateContract(ContratVie contract)
    {
        lock (_lock)
        {
            if (!_contracts.ContainsKey(contract.Id))
            {
                throw new InvalidOperationException($"Contract {contract.Id} not found.");
            }

            // Retirer les anciennes références de l'index
            var oldContract = _contracts[contract.Id];
            foreach (var reference in oldContract.ExternalReferences)
            {
                var key = (oldContract.ClientId, reference.SourceSystem, reference.ReferenceType, reference.Value);
                _externalReferenceIndex.Remove(key);
            }

            // Vérifier les doublons des nouvelles références
            foreach (var reference in contract.ExternalReferences)
            {
                var key = (contract.ClientId, reference.SourceSystem, reference.ReferenceType, reference.Value);
                if (_externalReferenceIndex.ContainsKey(key))
                {
                    throw new InvalidOperationException(
                        $"External reference ({reference.SourceSystem}, {reference.ReferenceType}, {reference.Value}) already exists for ClientId {contract.ClientId}.");
                }
            }

            _contracts[contract.Id] = contract;

            // Réindexer les nouvelles références
            foreach (var reference in contract.ExternalReferences)
            {
                var key = (contract.ClientId, reference.SourceSystem, reference.ReferenceType, reference.Value);
                _externalReferenceIndex[key] = contract.Id;
            }
        }
    }

    public ContratVie? GetContract(Guid contractId)
    {
        lock (_lock)
        {
            return _contracts.GetValueOrDefault(contractId);
        }
    }

    public IReadOnlyList<ContratVie> GetContractsByClientId(Guid clientId)
    {
        lock (_lock)
        {
            return _contracts.Values
                .Where(c => c.ClientId == clientId)
                .ToList();
        }
    }

    public ContratVie? FindContractByExternalReference(
        Guid clientId,
        SourceSystem sourceSystem,
        ReferenceType referenceType,
        string value)
    {
        lock (_lock)
        {
            var key = (clientId, sourceSystem, referenceType, value);
            if (_externalReferenceIndex.TryGetValue(key, out var contractId))
            {
                return _contracts.GetValueOrDefault(contractId);
            }
            return null;
        }
    }

    public void Clear()
    {
        lock (_lock)
        {
            _clients.Clear();
            _contracts.Clear();
            _externalReferenceIndex.Clear();
        }
    }
}
