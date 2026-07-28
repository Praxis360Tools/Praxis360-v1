using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Praxis360.Domain.Types;
using Praxis360_v1.Application.Interfaces;
using Praxis360_v1.Application.Models;
using Praxis360_v1.Domain.Aggregates;
using Praxis360_v1.Domain.Entities;
using Praxis360_v1.Infrastructure.Persistence;
using Praxis360_v1.Infrastructure.Persistence.Mappers;

namespace Praxis360_v1.Infrastructure.Services;

/// <summary>
/// Service de persistance atomique EF Core/SQLite pour clients et contrats BRIO.
/// </summary>
public sealed class EfCoreBrioPersistenceService : IBrioPersistenceService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;
    private readonly ILogger<EfCoreBrioPersistenceService> _logger;

    public EfCoreBrioPersistenceService(
        IDbContextFactory<AppDbContext> contextFactory,
        ILogger<EfCoreBrioPersistenceService> logger)
    {
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<BrioPersistenceBatchResult> PersistNewClientWithContractsAsync(
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
            return BrioPersistenceBatchResult.ValidationFailure(
                new[]
                {
                    new BrioPersistenceIssue
                    {
                        Severity = BrioPersistenceSeverity.Error,
                        Message = "Au moins un contrat doit être fourni."
                    }
                },
                client.Id);
        }

        // Validation : tous les contrats doivent appartenir au client canonique
        foreach (var contract in contracts)
        {
            if (contract.ClientId != client.Id)
            {
                return BrioPersistenceBatchResult.ValidationFailure(
                    new[]
                    {
                        new BrioPersistenceIssue
                        {
                            Severity = BrioPersistenceSeverity.Error,
                            Message = $"Le contrat {contract.Id} a un ClientId ({contract.ClientId}) différent du client canonique ({client.Id})."
                        }
                    },
                    client.Id);
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
                    return BrioPersistenceBatchResult.ValidationFailure(
                        new[]
                        {
                            new BrioPersistenceIssue
                            {
                                Severity = BrioPersistenceSeverity.Error,
                                Message = $"Le lot contient des doublons de référence externe."
                            }
                        },
                        client.Id);
                }
            }
        }

        cancellationToken.ThrowIfCancellationRequested();

        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // Ajouter le client
            var clientEntity = ClientPersistenceMapper.ToEntity(client);
            context.Clients.Add(clientEntity);

            // Ajouter tous les contrats
            foreach (var contract in contracts)
            {
                var contractEntity = ContractPersistenceMapper.ToEntity(contract);
                context.Contracts.Add(contractEntity);
            }

            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            var persistedIds = contracts.Select(c => c.Id).ToList();
            return BrioPersistenceBatchResult.Success(
                client.Id,
                clientWasPersisted: true,
                persistedIds);
        }
        catch (OperationCanceledException)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
        catch (DbUpdateException ex) when (IsSqliteUniqueConstraintViolation(ex))
        {
            await transaction.RollbackAsync(CancellationToken.None);
            _logger.LogWarning("Violation de contrainte unique lors de la persistance atomique du client");
            return BrioPersistenceBatchResult.DuplicateExternalReference(
                affectedContractId: null,
                message: "Une référence externe existe déjà.",
                clientId: client.Id);
        }
        catch (DbUpdateException ex)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            _logger.LogError(ex, "Erreur de persistance lors de l'enregistrement atomique");
            return BrioPersistenceBatchResult.PersistenceFailure(
                "Échec de la persistance en base de données.",
                client.Id);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            _logger.LogError(ex, "Erreur inattendue lors de la persistance atomique");
            return BrioPersistenceBatchResult.PersistenceFailure(
                "Erreur inattendue lors de la persistance.",
                client.Id);
        }
    }

    public async Task<BrioPersistenceBatchResult> PersistContractsForExistingClientAsync(
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
            return BrioPersistenceBatchResult.ValidationFailure(
                new[]
                {
                    new BrioPersistenceIssue
                    {
                        Severity = BrioPersistenceSeverity.Error,
                        Message = "Au moins un contrat doit être fourni."
                    }
                },
                clientId);
        }

        // Validation : tous les contrats doivent appartenir au clientId canonique
        foreach (var contract in contracts)
        {
            if (contract.ClientId != clientId)
            {
                return BrioPersistenceBatchResult.ValidationFailure(
                    new[]
                    {
                        new BrioPersistenceIssue
                        {
                            Severity = BrioPersistenceSeverity.Error,
                            Message = $"Le contrat {contract.Id} a un ClientId ({contract.ClientId}) différent du clientId canonique ({clientId})."
                        }
                    },
                    clientId);
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
                    return BrioPersistenceBatchResult.ValidationFailure(
                        new[]
                        {
                            new BrioPersistenceIssue
                            {
                                Severity = BrioPersistenceSeverity.Error,
                                Message = $"Le lot contient des doublons de référence externe."
                            }
                        },
                        clientId);
                }
            }
        }

        cancellationToken.ThrowIfCancellationRequested();

        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        // Vérifier que le client existe
        var clientExists = await context.Clients.AnyAsync(c => c.Id == clientId, cancellationToken);
        if (!clientExists)
        {
            return BrioPersistenceBatchResult.ClientNotFound(clientId);
        }

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // Ajouter tous les contrats
            foreach (var contract in contracts)
            {
                var contractEntity = ContractPersistenceMapper.ToEntity(contract);
                context.Contracts.Add(contractEntity);
            }

            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            var persistedIds = contracts.Select(c => c.Id).ToList();
            return BrioPersistenceBatchResult.Success(
                clientId,
                clientWasPersisted: false,
                persistedIds);
        }
        catch (OperationCanceledException)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
        catch (DbUpdateException ex) when (IsSqliteUniqueConstraintViolation(ex))
        {
            await transaction.RollbackAsync(CancellationToken.None);
            _logger.LogWarning("Violation de contrainte unique lors de la persistance atomique pour un client existant");
            return BrioPersistenceBatchResult.DuplicateExternalReference(
                affectedContractId: null,
                message: "Une référence externe existe déjà.",
                clientId: clientId);
        }
        catch (DbUpdateException ex)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            _logger.LogError(ex, "Erreur de persistance lors de l'enregistrement atomique pour un client existant");
            return BrioPersistenceBatchResult.PersistenceFailure(
                "Échec de la persistance en base de données.",
                clientId);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            _logger.LogError(ex, "Erreur inattendue lors de la persistance atomique pour un client existant");
            return BrioPersistenceBatchResult.PersistenceFailure(
                "Erreur inattendue lors de la persistance.",
                clientId);
        }
    }

    private static bool IsSqliteUniqueConstraintViolation(DbUpdateException exception)
    {
        var sqliteException = exception.GetBaseException() as SqliteException;
        if (sqliteException is null)
            return false;

        // SQLITE_CONSTRAINT_UNIQUE = 2067
        return sqliteException.SqliteExtendedErrorCode == 2067;
    }
}
