using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Praxis360_v1.Models;
using Praxis360_v1.Domain.Aggregates;
using Praxis360_v1.Domain.Entities;
using Praxis360_v1.Domain.ValueObjects;
using Praxis360.Domain.Types;
using Praxis360_v1.Domain.Types;
using Praxis360_v1.Application.Interfaces;

namespace Praxis360_v1.Services;

public class SituationAssuranceVieService
{
    private readonly IClientRepository _clientRepository;
    private readonly IContractRepository _contractRepository;

    public SituationAssuranceVieService(IClientRepository clientRepository, IContractRepository contractRepository)
    {
        _clientRepository = clientRepository ?? throw new ArgumentNullException(nameof(clientRepository));
        _contractRepository = contractRepository ?? throw new ArgumentNullException(nameof(contractRepository));
    }

    public async Task<SituationAssuranceVieReadModel?> GetSituationForClientAsync(Guid clientId)
    {
        var client = await _clientRepository.GetByIdAsync(clientId);

        if (client is null)
            return null;

        var contracts = await _contractRepository.GetByClientIdAsync(clientId);
        var contractsList = contracts.ToList();

        int totalContracts = contractsList.Count;
        int currentContracts = contractsList.Count(c => IsContractConsideredCurrent(c.Status));

        // Financial indicators are not available from current Domain — remain null
        Money? reserve = null;
        Money? capitalATerme = null;
        Money? capitalDeces = null;
        Money? revenuGaranti = null;

        var contractSummaries = contractsList.Select(c => MapToContractReadModel(c)).ToList();

        return new SituationAssuranceVieReadModel
        {
            ClientId = client.Id,
            ClientDisplayName = $"{client.FirstName} {client.LastName}",
            TotalContracts = totalContracts,
            CurrentContracts = currentContracts,
            ReserveAcquise = reserve,
            CapitalATerme = capitalATerme,
            CapitalDeces = capitalDeces,
            RevenuGaranti = revenuGaranti,
            Contracts = contractSummaries
        };
    }

    public async Task<SituationAssuranceVieLoadResult> GetSituationForDefaultClientAsync()
    {
        var clients = await _clientRepository.GetAllAsync();
        var clientsList = clients.ToList();

        if (clientsList.Count == 0)
        {
            return new SituationAssuranceVieLoadResult(null, SituationAssuranceVieLoadStatus.NoClientsAvailable);
        }

        if (clientsList.Count == 1)
        {
            var client = clientsList[0];
            var situation = await GetSituationForClientAsync(client.Id);

            if (situation is null)
            {
                return new SituationAssuranceVieLoadResult(null, SituationAssuranceVieLoadStatus.ClientNotFound);
            }

            return new SituationAssuranceVieLoadResult(situation, SituationAssuranceVieLoadStatus.ClientLoaded);
        }

        // Multiple clients require explicit selection
        return new SituationAssuranceVieLoadResult(null, SituationAssuranceVieLoadStatus.MultipleClientsRequireSelection);
    }

    private static bool IsContractConsideredCurrent(ContractStatus status)
    {
        return status == ContractStatus.Active ||
               status == ContractStatus.PaidUp ||
               status == ContractStatus.Suspended;
    }

    private static SituationAssuranceVieContractReadModel MapToContractReadModel(ContratVie contract)
    {
        string contractTypeLabel = MapContractTypeToLabel(contract.Type);
        string statusLabel = MapContractStatusToLabel(contract.Status);
        string statusCssClass = MapContractStatusToCssClass(contract.Status);
        string description = MapContractTypeToDescription(contract.Type);

        string? insurerKey = contract.Insurer?.Key;
        string insurerDisplayName = DetermineInsurerDisplayName(contract);

        var guaranteeLabels = contract.Garanties
            .Select(g => MapGarantieTypeToLabel(g))
            .ToList();

        return new SituationAssuranceVieContractReadModel(
            contract.Number.Value,
            contract.Type,
            contractTypeLabel,
            insurerKey,
            insurerDisplayName,
            contract.Status,
            statusLabel,
            statusCssClass,
            guaranteeLabels,
            description
        );
    }

    private static string DetermineInsurerDisplayName(ContratVie contract)
    {
        // First preference: use the Insurer aggregate's DisplayName if it exists
        if (contract.Insurer is not null)
        {
            return contract.Insurer.DisplayName;
        }

        // Fallback: find the most recent BRIO provenance with a raw insurer name
        var brioProvenance = contract.Provenances
            .Where(p => p.SourceSystem == SourceSystem.Brio)
            .OrderByDescending(p => p.ImportedAtUtc)
            .FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(brioProvenance?.RawInsurerName))
        {
            return brioProvenance.RawInsurerName;
        }

        // No insurer information available
        return "Compagnie non disponible";
    }

    private static string MapContractStatusToLabel(ContractStatus status)
    {
        return status switch
        {
            ContractStatus.Draft => "Brouillon",
            ContractStatus.Active => "En cours",
            ContractStatus.PaidUp => "Réduite",
            ContractStatus.Suspended => "Suspendue",
            ContractStatus.Terminated => "Terminée",
            ContractStatus.Matured => "Arrivée à terme",
            _ => "Statut non disponible"
        };
    }

    private static string MapContractStatusToCssClass(ContractStatus status)
    {
        return status switch
        {
            ContractStatus.Draft => "status-draft",
            ContractStatus.Active => "status-active",
            ContractStatus.PaidUp => "status-paid-up",
            ContractStatus.Suspended => "status-suspended",
            ContractStatus.Terminated => "status-terminated",
            ContractStatus.Matured => "status-matured",
            _ => "status-unknown"
        };
    }

    private static string MapContractTypeToLabel(ContractType type)
    {
        return type switch
        {
            ContractType.PLCI => "PLCI",
            ContractType.EIP => "EIP",
            ContractType.PensionAgreement => "Convention de pension",
            ContractType.IndividualLifeInsurance => "Assurance vie individuelle",
            _ => "Type de contrat non disponible"
        };
    }

    private static string MapContractTypeToDescription(ContractType type)
    {
        return type switch
        {
            ContractType.PLCI => "Plan de pension complémentaire pour indépendant.",
            ContractType.EIP => "Engagement individuel de pension.",
            ContractType.PensionAgreement => "Convention de pension.",
            ContractType.IndividualLifeInsurance => "Contrat d'assurance vie individuelle.",
            _ => "Description non disponible."
        };
    }

    private static string MapGarantieTypeToLabel(Garantie garantie)
    {
        return garantie.GarantieType switch
        {
            TypeGarantie.IncomeProtection => "Revenu garanti",
            TypeGarantie.DeathBenefit => "Capital décès",
            TypeGarantie.PremiumWaiver => "Exonération de prime",
            TypeGarantie.Other => !string.IsNullOrWhiteSpace(garantie.Label) ? garantie.Label : "Autre garantie",
            _ => !string.IsNullOrWhiteSpace(garantie.Label) ? garantie.Label : "Garantie non disponible"
        };
    }
}
