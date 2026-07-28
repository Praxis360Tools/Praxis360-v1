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
                },
                client.Id));
        }

        // Validation : tous les contrats doivent appartenir au client canonique
        foreach (var contract in contracts)
        {
            if (contract.ClientId != client.Id)
            {
                return Task.FromResult(BrioPersistenceBatchResult.ValidationFailure(
                    new[]
                    {
                        new BrioPersistenceIssue
                        {
                            Severity = BrioPersistenceSeverity.Error,
                            Message = $"Le contrat {contract.Id} a un ClientId ({contract.ClientId}) différent du client canonique ({client.Id})."
                        }
                    },
                    client.Id));
            }
        }

        // Validation : pas de doublons internes dans le lot (clé canonique : client.Id)
        var duplicateCheck = new HashSet<(Guid ClientId, SourceSystem SourceSystem, ReferenceType ReferenceType, string Value)>();
        foreach (var contract in contracts)
        {
            foreach (var reference in contract.ExternalReferences)
            {
                var key = (client.Id, reference.SourceSystem, reference.ReferenceType, reference.Value);
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
                        client.Id));
                }
            }
        }

        cancellationToken.ThrowIfCancellationRequested();

        // Atomicité réelle via ExecuteAtomicBatch
        try
        {
            _store.ExecuteAtomicBatch(
                clientToAdd: client,
                contractsToAdd: contracts,
                out var clientWasPersisted,
                out var persistedIds);

            return Task.FromResult(BrioPersistenceBatchResult.Success(
                client.Id,
                clientWasPersisted: clientWasPersisted,
                persistedIds));
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
        {
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

        // Validation : tous les contrats doivent appartenir au clientId canonique
        foreach (var contract in contracts)
        {
            if (contract.ClientId != clientId)
            {
                return Task.FromResult(BrioPersistenceBatchResult.ValidationFailure(
                    new[]
                    {
                        new BrioPersistenceIssue
                        {
                            Severity = BrioPersistenceSeverity.Error,
                            Message = $"Le contrat {contract.Id} a un ClientId ({contract.ClientId}) différent du clientId canonique ({clientId})."
                        }
                    },
                    clientId));
            }
        }

        // Validation : pas de doublons internes dans le lot (clé canonique : clientId)
        var duplicateCheck = new HashSet<(Guid ClientId, SourceSystem SourceSystem, ReferenceType ReferenceType, string Value)>();
        foreach (var contract in contracts)
        {
            foreach (var reference in contract.ExternalReferences)
            {
                var key = (clientId, reference.SourceSystem, reference.ReferenceType, reference.Value);
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

        // Atomicité réelle via ExecuteAtomicBatch (client déjà présent)
        try
        {
            _store.ExecuteAtomicBatch(
                clientToAdd: null,
                contractsToAdd: contracts,
                out var clientWasPersisted,
                out var persistedIds);

            return Task.FromResult(BrioPersistenceBatchResult.Success(
                clientId,
                clientWasPersisted: clientWasPersisted,
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
