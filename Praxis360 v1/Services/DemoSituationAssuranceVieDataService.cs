using System;
using System.Collections.Generic;
using System.Linq;
using Praxis360_v1.Domain.Aggregates;
using Praxis360_v1.Domain.Entities;
using Praxis360_v1.Domain.ValueObjects;
using Praxis360.Domain.Types;
using Praxis360_v1.Domain.Types;

namespace Praxis360_v1.Services;

public class DemoSituationAssuranceVieDataService
{
    private readonly IReadOnlyList<Client> _clients;
    private readonly IReadOnlyList<ContratVie> _contracts;

    public DemoSituationAssuranceVieDataService()
    {
        // Deterministic GUIDs for demo data
        var clientId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        var client = new Client(
            clientId,
            "Jean",
            "Dupont",
            new DateOnly(1980, 1, 1),
            Praxis360.Domain.Types.Language.French,
            null);

        // Contract 1: PLCI Vivium - Active - IncomeProtection
        var contract1 = new ContratVie(
            Guid.Parse("00000000-0000-0000-0000-000000000101"),
            new ContractNumber("C-1001"),
            ContractType.PLCI,
            ContractStatus.Active,
            client.Id,
            new Insurer("VIVIUM", "Vivium"));

        var garantie1 = new Garantie(
            Guid.Parse("00000000-0000-0000-0000-000000001001"),
            TypeGarantie.IncomeProtection,
            null,
            null,
            null);

        contract1.AddGarantie(garantie1);

        // Contract 2: EIP AG - Active - DeathBenefit
        var contract2 = new ContratVie(
            Guid.Parse("00000000-0000-0000-0000-000000000102"),
            new ContractNumber("C-1002"),
            ContractType.EIP,
            ContractStatus.Active,
            client.Id,
            new Insurer("AG", "AG"));

        var garantie2 = new Garantie(
            Guid.Parse("00000000-0000-0000-0000-000000001002"),
            TypeGarantie.DeathBenefit,
            null,
            null,
            null);

        contract2.AddGarantie(garantie2);

        // Contract 3: PLCI NN - PaidUp - No guarantees
        var contract3 = new ContratVie(
            Guid.Parse("00000000-0000-0000-0000-000000000103"),
            new ContractNumber("C-1003"),
            ContractType.PLCI,
            ContractStatus.PaidUp,
            client.Id,
            new Insurer("NN", "NN"));

        _clients = new[] { client };
        _contracts = new[] { contract1, contract2, contract3 };
    }

    public Client? GetDefaultClient()
    {
        return _clients.FirstOrDefault();
    }

    public Client? GetClient(Guid clientId)
    {
        return _clients.FirstOrDefault(c => c.Id == clientId);
    }

    public IReadOnlyList<ContratVie> GetContractsForClient(Guid clientId)
    {
        return _contracts.Where(c => c.ClientId == clientId).ToList();
    }
}
