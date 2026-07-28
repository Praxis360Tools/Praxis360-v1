using Praxis360.Domain.Types;
using Praxis360_v1.Application.Interfaces;
using Praxis360_v1.Application.Models;
using Praxis360_v1.Domain.Aggregates;
using Praxis360_v1.Domain.Entities;

namespace Praxis360_v1.Infrastructure.InMemory;

/// <summary>
/// Implémentation InMemory du service de persistance atomique BRIO.
/// </summary>
public sealed class InMemoryBrioPersistenceService : IBrioPersistenceService
{
    private readonly InMemoryPraxis360Store _store;

    public InMemoryBrioPersistenceService(InMemoryPraxis360Store store)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
    }

    public Task<BrioPersistenceBatchResult> PersistNewClientWithContractsAsync(
        Client client,
        IReadOnlyCollection<ContratVie> contracts,
        CancellationToken cancellationToken = default)
    {
        if (client is null)
            throw new ArgumentNullException(nameof(client));
        if (contracts is null)
            throw new ArgumentNullException(nameof(contracts));

        cancellationToken.ThrowIfCancellationRequested();

        // Validation : au moins un contrat
        if (contracts.Count == 0)
        {
            return Task.FromResult(BrioPersistenceBatchResult.ValidationFailure(
                new[]
                {
                    new BrioPersistenceIssue
                    {
                        Severity = BrioPersistenceSeverity.Error,
                        Message = "Au moins un contrat doit être fourni."
                    }
                }));
        }

        // Validation : pas de doublons internes dans le lot
        var duplicateCheck = new HashSet<(Guid ClientId, SourceSystem SourceSystem, ReferenceType ReferenceType, string Value)>();
        foreach (var contract in contracts)
        {
            foreach (var reference in contract.ExternalReferences)
            {
                var key = (contract.ClientId, reference.SourceSystem, reference.ReferenceType, reference.Value);
                if (!duplicateCheck.Add(key))
                {
                    return Task.FromResult(BrioPersistenceBatchResult.ValidationFailure(
                        new[]
                        {
                            new BrioPersistenceIssue
                            {
                                Severity = BrioPersistenceSeverity.Error,
                                Message = $"Le lot contient des doublons de référence externe : {reference.SourceSystem}/{reference.ReferenceType}/{reference.Value}."
                            }
                        }));
                }
            }
        }

        cancellationToken.ThrowIfCancellationRequested();

        // Atomicité simulée : tout ou rien
        try
        {
            _store.AddClient(client);

            var persistedIds = new List<Guid>();
            foreach (var contract in contracts)
            {
                _store.AddContract(contract);
                persistedIds.Add(contract.Id);
            }

            return Task.FromResult(BrioPersistenceBatchResult.Success(
                client.Id,
                clientWasPersisted: true,
                persistedIds));
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
        {
            // En cas d'échec, le store InMemory garantit que rien n'a été persisté
            return Task.FromResult(BrioPersistenceBatchResult.DuplicateExternalReference(
                affectedContractId: null,
                message: "Une référence externe existe déjà.",
                clientId: client.Id));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return Task.FromResult(BrioPersistenceBatchResult.PersistenceFailure(
                $"Erreur lors de la persistance : {ex.Message}",
                client.Id));
        }
    }

    public Task<BrioPersistenceBatchResult> PersistContractsForExistingClientAsync(
        Guid clientId,
        IReadOnlyCollection<ContratVie> contracts,
        CancellationToken cancellationToken = default)
    {
        if (contracts is null)
            throw new ArgumentNullException(nameof(contracts));

        cancellationToken.ThrowIfCancellationRequested();

        // Validation : au moins un contrat
        if (contracts.Count == 0)
        {
            return Task.FromResult(BrioPersistenceBatchResult.ValidationFailure(
                new[]
                {
                    new BrioPersistenceIssue
                    {
                        Severity = BrioPersistenceSeverity.Error,
                        Message = "Au moins un contrat doit être fourni."
                    }
                },
                clientId));
        }

        // Vérifier que le client existe
        var existingClient = _store.GetClient(clientId);
        if (existingClient is null)
        {
            return Task.FromResult(BrioPersistenceBatchResult.ClientNotFound(clientId));
        }

        // Validation : pas de doublons internes dans le lot
        var duplicateCheck = new HashSet<(Guid ClientId, SourceSystem SourceSystem, ReferenceType ReferenceType, string Value)>();
        foreach (var contract in contracts)
        {
            foreach (var reference in contract.ExternalReferences)
            {
                var key = (contract.ClientId, reference.SourceSystem, reference.ReferenceType, reference.Value);
                if (!duplicateCheck.Add(key))
                {
                    return Task.FromResult(BrioPersistenceBatchResult.ValidationFailure(
                        new[]
                        {
                            new BrioPersistenceIssue
                            {
                                Severity = BrioPersistenceSeverity.Error,
                                Message = $"Le lot contient des doublons de référence externe : {reference.SourceSystem}/{reference.ReferenceType}/{reference.Value}."
                            }
                        },
                        clientId));
                }
            }
        }

        cancellationToken.ThrowIfCancellationRequested();

        // Atomicité simulée : tout ou rien
        try
        {
            var persistedIds = new List<Guid>();
            foreach (var contract in contracts)
            {
                _store.AddContract(contract);
                persistedIds.Add(contract.Id);
            }

            return Task.FromResult(BrioPersistenceBatchResult.Success(
                clientId,
                clientWasPersisted: false,
                persistedIds));
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
        {
            return Task.FromResult(BrioPersistenceBatchResult.DuplicateExternalReference(
                affectedContractId: null,
                message: "Une référence externe existe déjà.",
                clientId: clientId));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return Task.FromResult(BrioPersistenceBatchResult.PersistenceFailure(
                $"Erreur lors de la persistance : {ex.Message}",
                clientId));
        }
    }
}
