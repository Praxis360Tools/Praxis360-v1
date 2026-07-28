using Praxis360.Domain.Types;
using Praxis360_v1.Application.Models;
using Praxis360_v1.Domain.Aggregates;
using Praxis360_v1.Domain.Entities;
using Praxis360_v1.Domain.ValueObjects;
using Praxis360_v1.Infrastructure.InMemory;

namespace Praxis360_v1.Tests.Infrastructure.InMemory;

public sealed class InMemoryBrioPersistenceServiceTests
{
    [Fact]
    public async Task PersistNewClientWithContractsAsync_EmptyContracts_ShouldReturnValidationFailureWithClientId()
    {
        var store = new InMemoryPraxis360Store();
        var service = new InMemoryBrioPersistenceService(store);
        var client = new Client(Guid.NewGuid(), "Jean", "Dupont", new DateOnly(1980, 5, 15), Language.French, null);

        var result = await service.PersistNewClientWithContractsAsync(client, Array.Empty<ContratVie>());

        Assert.Equal(BrioPersistenceOutcome.ValidationFailure, result.Outcome);
        Assert.Equal(client.Id, result.ClientId);
        Assert.False(result.ClientWasPersisted);
        Assert.Empty(result.PersistedContractIds);
        Assert.Null(store.GetClient(client.Id));
    }

    [Fact]
    public async Task PersistNewClientWithContractsAsync_WrongClientId_ShouldReturnValidationFailure()
    {
        var store = new InMemoryPraxis360Store();
        var service = new InMemoryBrioPersistenceService(store);
        var client = new Client(Guid.NewGuid(), "WrongClient", "Test", new DateOnly(1985, 1, 1), Language.French, null);
        var wrongClientId = Guid.NewGuid();
        var contract = new ContratVie(Guid.NewGuid(), new ContractNumber("POL-WRONG"), ContractType.PLCI, ContractStatus.Active, wrongClientId, null);
        contract.AddExternalReference(new ExternalReference(SourceSystem.Brio, ReferenceType.PolicyNumber, "REF-WRONG"));

        var result = await service.PersistNewClientWithContractsAsync(client, new[] { contract });

        Assert.Equal(BrioPersistenceOutcome.ValidationFailure, result.Outcome);
        Assert.Equal(client.Id, result.ClientId);
        Assert.False(result.ClientWasPersisted);
        Assert.Empty(result.PersistedContractIds);
        Assert.Null(store.GetClient(client.Id));
        Assert.Null(store.GetContract(contract.Id));
    }

    [Fact]
    public async Task PersistContractsForExistingClientAsync_WrongClientId_ShouldReturnValidationFailure()
    {
        var store = new InMemoryPraxis360Store();
        var service = new InMemoryBrioPersistenceService(store);
        var client = new Client(Guid.NewGuid(), "ExistingClient", "Test", new DateOnly(1985, 1, 1), Language.French, null);
        var initialContract = new ContratVie(Guid.NewGuid(), new ContractNumber("POL-INIT"), ContractType.PLCI, ContractStatus.Active, client.Id, null);
        initialContract.AddExternalReference(new ExternalReference(SourceSystem.Brio, ReferenceType.PolicyNumber, "REF-INIT"));

        await service.PersistNewClientWithContractsAsync(client, new[] { initialContract });

        var wrongClientId = Guid.NewGuid();
        var badContract = new ContratVie(Guid.NewGuid(), new ContractNumber("POL-BAD"), ContractType.EIP, ContractStatus.Active, wrongClientId, null);
        badContract.AddExternalReference(new ExternalReference(SourceSystem.Brio, ReferenceType.PolicyNumber, "REF-BAD"));

        var result = await service.PersistContractsForExistingClientAsync(client.Id, new[] { badContract });

        Assert.Equal(BrioPersistenceOutcome.ValidationFailure, result.Outcome);
        Assert.Equal(client.Id, result.ClientId);
        Assert.False(result.ClientWasPersisted);
        Assert.Empty(result.PersistedContractIds);
        Assert.NotNull(store.GetClient(client.Id));
        Assert.Null(store.GetContract(badContract.Id));
    }

    [Fact]
    public async Task PersistNewClientWithContractsAsync_RollbackOnThirdContractFailure_ShouldLeaveStoreUnchanged()
    {
        var store = new InMemoryPraxis360Store();
        var service = new InMemoryBrioPersistenceService(store);
        var client = new Client(Guid.NewGuid(), "Atomic", "Test", new DateOnly(1985, 1, 1), Language.French, null);

        var contract1 = new ContratVie(Guid.NewGuid(), new ContractNumber("POL-1"), ContractType.PLCI, ContractStatus.Active, client.Id, null);
        contract1.AddExternalReference(new ExternalReference(SourceSystem.Brio, ReferenceType.PolicyNumber, "REF-1"));

        var contract2 = new ContratVie(Guid.NewGuid(), new ContractNumber("POL-2"), ContractType.EIP, ContractStatus.Active, client.Id, null);
        contract2.AddExternalReference(new ExternalReference(SourceSystem.Brio, ReferenceType.PolicyNumber, "REF-2"));

        var existingClient = new Client(Guid.NewGuid(), "Existing", "Client", new DateOnly(1990, 1, 1), Language.French, null);
        var existingContract = new ContratVie(Guid.NewGuid(), new ContractNumber("POL-EXIST"), ContractType.PLCI, ContractStatus.Active, existingClient.Id, null);
        existingContract.AddExternalReference(new ExternalReference(SourceSystem.Brio, ReferenceType.PolicyNumber, "REF-EXIST"));
        await service.PersistNewClientWithContractsAsync(existingClient, new[] { existingContract });

        var preExistingContract = new ContratVie(Guid.NewGuid(), new ContractNumber("POL-PRE"), ContractType.IndividualLifeInsurance, ContractStatus.Active, client.Id, null);
        preExistingContract.AddExternalReference(new ExternalReference(SourceSystem.Brio, ReferenceType.PolicyNumber, "REF-PRE"));
        await service.PersistNewClientWithContractsAsync(client, new[] { preExistingContract });

        var newContract1 = new ContratVie(Guid.NewGuid(), new ContractNumber("POL-NEW1"), ContractType.PLCI, ContractStatus.Active, client.Id, null);
        newContract1.AddExternalReference(new ExternalReference(SourceSystem.Brio, ReferenceType.PolicyNumber, "REF-NEW1"));

        var newContract2 = new ContratVie(Guid.NewGuid(), new ContractNumber("POL-NEW2"), ContractType.EIP, ContractStatus.Active, client.Id, null);
        newContract2.AddExternalReference(new ExternalReference(SourceSystem.Brio, ReferenceType.PolicyNumber, "REF-NEW2"));

        var newContract3 = new ContratVie(Guid.NewGuid(), new ContractNumber("POL-NEW3"), ContractType.IndividualLifeInsurance, ContractStatus.Active, client.Id, null);
        newContract3.AddExternalReference(new ExternalReference(SourceSystem.Brio, ReferenceType.PolicyNumber, "REF-PRE"));

        var result = await service.PersistContractsForExistingClientAsync(client.Id, new[] { newContract1, newContract2, newContract3 });

        Assert.NotEqual(BrioPersistenceOutcome.Success, result.Outcome);
        Assert.NotNull(store.GetClient(client.Id));
        Assert.NotNull(store.GetContract(preExistingContract.Id));
        Assert.Null(store.GetContract(newContract1.Id));
        Assert.Null(store.GetContract(newContract2.Id));
        Assert.Null(store.GetContract(newContract3.Id));
        Assert.NotNull(store.GetClient(existingClient.Id));
        Assert.NotNull(store.GetContract(existingContract.Id));
    }

    [Fact]
    public async Task PersistContractsForExistingClientAsync_RollbackOnFailure_ShouldLeaveClientIntact()
    {
        var store = new InMemoryPraxis360Store();
        var service = new InMemoryBrioPersistenceService(store);
        var client = new Client(Guid.NewGuid(), "Existing", "Client", new DateOnly(1985, 1, 1), Language.French, null);
        var initialContract = new ContratVie(Guid.NewGuid(), new ContractNumber("POL-INIT"), ContractType.PLCI, ContractStatus.Active, client.Id, null);
        initialContract.AddExternalReference(new ExternalReference(SourceSystem.Brio, ReferenceType.PolicyNumber, "REF-INIT"));

        await service.PersistNewClientWithContractsAsync(client, new[] { initialContract });

        var newContract1 = new ContratVie(Guid.NewGuid(), new ContractNumber("POL-NEW1"), ContractType.EIP, ContractStatus.Active, client.Id, null);
        newContract1.AddExternalReference(new ExternalReference(SourceSystem.Brio, ReferenceType.PolicyNumber, "REF-NEW1"));

        var newContract2 = new ContratVie(Guid.NewGuid(), new ContractNumber("POL-NEW2"), ContractType.IndividualLifeInsurance, ContractStatus.Active, client.Id, null);
        newContract2.AddExternalReference(new ExternalReference(SourceSystem.Brio, ReferenceType.PolicyNumber, "REF-INIT"));

        var result = await service.PersistContractsForExistingClientAsync(client.Id, new[] { newContract1, newContract2 });

        Assert.NotEqual(BrioPersistenceOutcome.Success, result.Outcome);
        Assert.NotNull(store.GetClient(client.Id));
        Assert.NotNull(store.GetContract(initialContract.Id));
        Assert.Null(store.GetContract(newContract1.Id));
        Assert.Null(store.GetContract(newContract2.Id));
    }
}
