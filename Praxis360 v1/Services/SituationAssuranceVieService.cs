using System;
using System.Collections.Generic;
using System.Linq;
using Praxis360_v1.Models;
using Praxis360_v1.Domain.Aggregates;
using Praxis360_v1.Domain.Entities;
using Praxis360_v1.Domain.ValueObjects;
using Praxis360.Domain.Types;

namespace Praxis360_v1.Services;

public class SituationAssuranceVieService
{
    private readonly DemoSituationAssuranceVieDataService _demoData;

    // Service constructed with demo data service injected
    public SituationAssuranceVieService(DemoSituationAssuranceVieDataService demoData)
    {
        _demoData = demoData ?? throw new ArgumentNullException(nameof(demoData));
    }

    public SituationAssuranceVieReadModel? GetSituationForClient(Guid clientId)
    {
        var client = _demoData.GetClient(clientId);

        if (client is null)
            return null;

        var contracts = _demoData.GetContractsForClient(clientId).ToList();

        int totalContracts = contracts.Count;

        // Determine active contracts by the explicit rule
        int currentContracts = contracts.Count(c => IsContractConsideredCurrent(c.Status));

        // Financial indicators are not available from current Domain — remain null
        Money? reserve = null;
        Money? capitalDeces = null;
        Money? revenuGaranti = null;

        var contractSummaries = contracts.Select(c => new SituationAssuranceVieContractReadModel(c.Number.Value, c.Status)).ToList();

        return new SituationAssuranceVieReadModel
        {
            ClientId = client.Id,
            ClientDisplayName = $"{client.FirstName} {client.LastName}",
            TotalContracts = totalContracts,
            CurrentContracts = currentContracts,
            ReserveAcquise = reserve,
            CapitalDeces = capitalDeces,
            RevenuGaranti = revenuGaranti,
            Contracts = contractSummaries
        };
    }

    public SituationAssuranceVieReadModel? GetDefaultSituation()
    {
        var client = _demoData.GetDefaultClient();

        if (client is null)
            return null;

        return GetSituationForClient(client.Id);
    }

    private static bool IsContractConsideredCurrent(ContractStatus status)
    {
        return status == ContractStatus.Active ||
               status == ContractStatus.PaidUp ||
               status == ContractStatus.Suspended;
    }
}
