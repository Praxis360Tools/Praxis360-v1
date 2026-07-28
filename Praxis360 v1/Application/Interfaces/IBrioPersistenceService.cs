using Praxis360_v1.Application.Models;
using Praxis360_v1.Domain.Aggregates;
using Praxis360_v1.Domain.Entities;

namespace Praxis360_v1.Application.Interfaces;

/// <summary>
/// Service applicatif de persistance atomique de clients et contrats BRIO.
/// </summary>
public interface IBrioPersistenceService
{
    /// <summary>
    /// Persiste un nouveau client et ses contrats de manière atomique.
    /// </summary>
    /// <remarks>
    /// Tout le lot est persisté dans une transaction unique.
    /// En cas d'échec sur un contrat, aucun contrat ni le client n'est enregistré.
    /// </remarks>
    Task<BrioPersistenceBatchResult> PersistNewClientWithContractsAsync(
        Client client,
        IReadOnlyCollection<ContratVie> contracts,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Persiste des contrats pour un client existant de manière atomique.
    /// </summary>
    /// <remarks>
    /// Tout le lot est persisté dans une transaction unique.
    /// En cas d'échec sur un contrat, aucun contrat du lot n'est enregistré.
    /// </remarks>
    Task<BrioPersistenceBatchResult> PersistContractsForExistingClientAsync(
        Guid clientId,
        IReadOnlyCollection<ContratVie> contracts,
        CancellationToken cancellationToken = default);
}
