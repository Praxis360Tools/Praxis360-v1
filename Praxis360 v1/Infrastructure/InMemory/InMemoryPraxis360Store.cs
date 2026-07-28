using Praxis360.Domain.Types;
using Praxis360_v1.Domain.Aggregates;
using Praxis360_v1.Domain.Entities;

namespace Praxis360_v1.Infrastructure.InMemory;

/// <summary>
/// Store en mémoire partagé pour les clients, contrats et références BRIO.
/// Permet aux repositories InMemory et au service de persistance de partager le même état.
/// Garantit l'atomicité des opérations batch via copy-on-write.
/// </summary>
public sealed class InMemoryPraxis360Store
{
    private readonly object _lock = new();
    private StoreSnapshot _currentSnapshot = new();

    /// <summary>
    /// Représente un snapshot immutable de l'état du store.
    /// </summary>
    private sealed class StoreSnapshot
    {
        public Dictionary<Guid, Client> Clients { get; init; } = new();
        public Dictionary<Guid, ContratVie> Contracts { get; init; } = new();
        public Dictionary<(Guid ClientId, SourceSystem SourceSystem, ReferenceType ReferenceType, string Value), Guid> ExternalReferenceIndex { get; init; } = new();

        public StoreSnapshot Clone()
        {
            return new StoreSnapshot
            {
                Clients = new Dictionary<Guid, Client>(Clients),
                Contracts = new Dictionary<Guid, ContratVie>(Contracts),
                ExternalReferenceIndex = new Dictionary<(Guid, SourceSystem, ReferenceType, string), Guid>(ExternalReferenceIndex)
            };
        }
    }

    /// <summary>
    /// Exécute une opération atomique batch (client + contrats).
    /// Garantit que soit tout est persisté, soit rien ne l'est.
    /// </summary>
    public void ExecuteAtomicBatch(
        Client? clientToAdd,
        IReadOnlyCollection<ContratVie> contractsToAdd,
        out bool clientWasPersisted,
        out List<Guid> persistedContractIds)
    {
        lock (_lock)
        {
            // Préparer un nouvel état à partir de l'état actuel
            var newSnapshot = _currentSnapshot.Clone();
            clientWasPersisted = false;
            persistedContractIds = new List<Guid>();

            // Étape 1 : Ajouter le client si fourni
            if (clientToAdd is not null)
            {
                if (newSnapshot.Clients.ContainsKey(clientToAdd.Id))
                {
                    throw new InvalidOperationException($"Client {clientToAdd.Id} already exists.");
                }
                newSnapshot.Clients[clientToAdd.Id] = clientToAdd;
                clientWasPersisted = true;
            }

            // Étape 2 : Ajouter tous les contrats
            foreach (var contract in contractsToAdd)
            {
                if (newSnapshot.Contracts.ContainsKey(contract.Id))
                {
                    throw new InvalidOperationException($"Contract {contract.Id} already exists.");
                }

                // Vérifier les doublons de références externes
                foreach (var reference in contract.ExternalReferences)
                {
                    var key = (contract.ClientId, reference.SourceSystem, reference.ReferenceType, reference.Value);
                    if (newSnapshot.ExternalReferenceIndex.ContainsKey(key))
                    {
                        throw new InvalidOperationException(
                            $"External reference ({reference.SourceSystem}, {reference.ReferenceType}, {reference.Value}) already exists for ClientId {contract.ClientId}.");
                    }
                }

                newSnapshot.Contracts[contract.Id] = contract;

                // Indexer les références externes
                foreach (var reference in contract.ExternalReferences)
                {
                    var key = (contract.ClientId, reference.SourceSystem, reference.ReferenceType, reference.Value);
                    newSnapshot.ExternalReferenceIndex[key] = contract.Id;
                }

                persistedContractIds.Add(contract.Id);
            }

            // Étape 3 : Publication atomique du nouvel état
            _currentSnapshot = newSnapshot;
        }
    }

    public void AddClient(Client client)
    {
        lock (_lock)
        {
            if (_currentSnapshot.Clients.ContainsKey(client.Id))
            {
                throw new InvalidOperationException($"Client {client.Id} already exists.");
            }

            var newSnapshot = _currentSnapshot.Clone();
            newSnapshot.Clients[client.Id] = client;
            _currentSnapshot = newSnapshot;
        }
    }

    public void UpdateClient(Client client)
    {
        lock (_lock)
        {
            if (!_currentSnapshot.Clients.ContainsKey(client.Id))
            {
                throw new InvalidOperationException($"Client {client.Id} not found.");
            }

            var newSnapshot = _currentSnapshot.Clone();
            newSnapshot.Clients[client.Id] = client;
            _currentSnapshot = newSnapshot;
        }
    }

    public Client? GetClient(Guid clientId)
    {
        lock (_lock)
        {
            return _currentSnapshot.Clients.GetValueOrDefault(clientId);
        }
    }

    public IReadOnlyList<Client> GetAllClients()
    {
        lock (_lock)
        {
            return _currentSnapshot.Clients.Values.ToList();
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
            if (_currentSnapshot.Contracts.ContainsKey(contract.Id))
            {
                throw new InvalidOperationException($"Contract {contract.Id} already exists.");
            }

            // Vérifier les doublons de références externes
            foreach (var reference in contract.ExternalReferences)
            {
                var key = (contract.ClientId, reference.SourceSystem, reference.ReferenceType, reference.Value);
                if (_currentSnapshot.ExternalReferenceIndex.ContainsKey(key))
                {
                    throw new InvalidOperationException(
                        $"External reference ({reference.SourceSystem}, {reference.ReferenceType}, {reference.Value}) already exists for ClientId {contract.ClientId}.");
                }
            }

            var newSnapshot = _currentSnapshot.Clone();
            newSnapshot.Contracts[contract.Id] = contract;

            // Indexer les références externes
            foreach (var reference in contract.ExternalReferences)
            {
                var key = (contract.ClientId, reference.SourceSystem, reference.ReferenceType, reference.Value);
                newSnapshot.ExternalReferenceIndex[key] = contract.Id;
            }

            _currentSnapshot = newSnapshot;
        }
    }

    public void UpdateContract(ContratVie contract)
    {
        lock (_lock)
        {
            if (!_currentSnapshot.Contracts.ContainsKey(contract.Id))
            {
                throw new InvalidOperationException($"Contract {contract.Id} not found.");
            }

            var newSnapshot = _currentSnapshot.Clone();

            // Retirer les anciennes références de l'index
            var oldContract = _currentSnapshot.Contracts[contract.Id];
            foreach (var reference in oldContract.ExternalReferences)
            {
                var key = (oldContract.ClientId, reference.SourceSystem, reference.ReferenceType, reference.Value);
                newSnapshot.ExternalReferenceIndex.Remove(key);
            }

            // Vérifier les doublons des nouvelles références
            foreach (var reference in contract.ExternalReferences)
            {
                var key = (contract.ClientId, reference.SourceSystem, reference.ReferenceType, reference.Value);
                if (newSnapshot.ExternalReferenceIndex.ContainsKey(key))
                {
                    throw new InvalidOperationException(
                        $"External reference ({reference.SourceSystem}, {reference.ReferenceType}, {reference.Value}) already exists for ClientId {contract.ClientId}.");
                }
            }

            newSnapshot.Contracts[contract.Id] = contract;

            // Réindexer les nouvelles références
            foreach (var reference in contract.ExternalReferences)
            {
                var key = (contract.ClientId, reference.SourceSystem, reference.ReferenceType, reference.Value);
                newSnapshot.ExternalReferenceIndex[key] = contract.Id;
            }

            _currentSnapshot = newSnapshot;
        }
    }

    public ContratVie? GetContract(Guid contractId)
    {
        lock (_lock)
        {
            return _currentSnapshot.Contracts.GetValueOrDefault(contractId);
        }
    }

    public IReadOnlyList<ContratVie> GetContractsByClientId(Guid clientId)
    {
        lock (_lock)
        {
            return _currentSnapshot.Contracts.Values
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
            if (_currentSnapshot.ExternalReferenceIndex.TryGetValue(key, out var contractId))
            {
                return _currentSnapshot.Contracts.GetValueOrDefault(contractId);
            }
            return null;
        }
    }

    public void Clear()
    {
        lock (_lock)
        {
            _currentSnapshot = new StoreSnapshot();
        }
    }
}
