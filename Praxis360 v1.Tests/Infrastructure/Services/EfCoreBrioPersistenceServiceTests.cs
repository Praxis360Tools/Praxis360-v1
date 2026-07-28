using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Praxis360.Domain.Types;
using Praxis360_v1.Application.Models;
using Praxis360_v1.Domain.Aggregates;
using Praxis360_v1.Domain.Entities;
using Praxis360_v1.Domain.ValueObjects;
using Praxis360_v1.Infrastructure.Persistence;
using Praxis360_v1.Infrastructure.Services;

namespace Praxis360_v1.Tests.Infrastructure.Services;

public sealed class EfCoreBrioPersistenceServiceTests : IDisposable
{
    private readonly string _tempDbPath;
    private readonly SqliteConnection _keepAliveConnection;
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public EfCoreBrioPersistenceServiceTests()
    {
        _tempDbPath = Path.Combine(Path.GetTempPath(), $"Praxis360_EfCorePersistence_{Guid.NewGuid():N}.db");
        var connectionString = $"Data Source={_tempDbPath};Foreign Keys=True";

        _keepAliveConnection = new SqliteConnection(connectionString);
        _keepAliveConnection.Open();

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_keepAliveConnection);

        _contextFactory = new TestDbContextFactory(optionsBuilder.Options);

        using var context = _contextFactory.CreateDbContext();
        context.Database.Migrate();
    }

    public void Dispose()
    {
        _keepAliveConnection?.Dispose();
        SqliteConnection.ClearAllPools();
        GC.Collect();
        GC.WaitForPendingFinalizers();

        if (File.Exists(_tempDbPath))
        {
            try { File.Delete(_tempDbPath); } catch { }
        }
    }

    [Fact]
    public async Task PersistNewClientWithContractsAsync_OneContract_ShouldSucceed()
    {
        var service = new EfCoreBrioPersistenceService(_contextFactory, NullLogger<EfCoreBrioPersistenceService>.Instance);
        var client = new Client(Guid.NewGuid(), "Jean", "Dupont", new DateOnly(1980, 5, 15), Language.French, null);
        var contract = new ContratVie(Guid.NewGuid(), new ContractNumber("POL-001"), ContractType.PLCI, ContractStatus.Active, client.Id, null);
        contract.AddExternalReference(new ExternalReference(SourceSystem.Brio, ReferenceType.PolicyNumber, "REF-001"));
        contract.AddProvenance(new ContractProvenance(SourceSystem.Brio, DateTime.UtcNow, "TestInsurer", new DateOnly(2025, 1, 27)));

        var result = await service.PersistNewClientWithContractsAsync(client, new[] { contract });

        Assert.Equal(BrioPersistenceOutcome.Success, result.Outcome);
        Assert.True(result.ClientWasPersisted);
        Assert.Single(result.PersistedContractIds);
        Assert.Equal(contract.Id, result.PersistedContractIds.First());
    }

    [Fact]
    public async Task PersistNewClientWithContractsAsync_MultipleContracts_ShouldSucceed()
    {
        var service = new EfCoreBrioPersistenceService(_contextFactory, NullLogger<EfCoreBrioPersistenceService>.Instance);
        var client = new Client(Guid.NewGuid(), "Marie", "Martin", new DateOnly(1985, 3, 10), Language.French, null);

        var contract1 = new ContratVie(Guid.NewGuid(), new ContractNumber("POL-100"), ContractType.PLCI, ContractStatus.Active, client.Id, null);
        contract1.AddExternalReference(new ExternalReference(SourceSystem.Brio, ReferenceType.PolicyNumber, "REF-100"));

        var contract2 = new ContratVie(Guid.NewGuid(), new ContractNumber("POL-200"), ContractType.EIP, ContractStatus.PaidUp, client.Id, null);
        contract2.AddExternalReference(new ExternalReference(SourceSystem.Brio, ReferenceType.PolicyNumber, "REF-200"));

        var result = await service.PersistNewClientWithContractsAsync(client, new[] { contract1, contract2 });

        Assert.Equal(BrioPersistenceOutcome.Success, result.Outcome);
        Assert.True(result.ClientWasPersisted);
        Assert.Equal(2, result.PersistedContractIds.Count);
    }

    [Fact]
    public async Task PersistContractsForExistingClientAsync_MultipleContracts_ShouldSucceed()
    {
        var service = new EfCoreBrioPersistenceService(_contextFactory, NullLogger<EfCoreBrioPersistenceService>.Instance);
        var client = new Client(Guid.NewGuid(), "Pierre", "Lemoine", new DateOnly(1975, 11, 5), Language.French, null);

        var initialContract = new ContratVie(Guid.NewGuid(), new ContractNumber("POL-INIT"), ContractType.IndividualLifeInsurance, ContractStatus.Active, client.Id, null);
        initialContract.AddExternalReference(new ExternalReference(SourceSystem.Brio, ReferenceType.PolicyNumber, "REF-INIT"));

        await service.PersistNewClientWithContractsAsync(client, new[] { initialContract });

        var newContract1 = new ContratVie(Guid.NewGuid(), new ContractNumber("POL-NEW1"), ContractType.PLCI, ContractStatus.Active, client.Id, null);
        newContract1.AddExternalReference(new ExternalReference(SourceSystem.Brio, ReferenceType.PolicyNumber, "REF-NEW1"));

        var newContract2 = new ContratVie(Guid.NewGuid(), new ContractNumber("POL-NEW2"), ContractType.EIP, ContractStatus.Active, client.Id, null);
        newContract2.AddExternalReference(new ExternalReference(SourceSystem.Brio, ReferenceType.PolicyNumber, "REF-NEW2"));

        var result = await service.PersistContractsForExistingClientAsync(client.Id, new[] { newContract1, newContract2 });

        Assert.Equal(BrioPersistenceOutcome.Success, result.Outcome);
        Assert.False(result.ClientWasPersisted);
        Assert.Equal(2, result.PersistedContractIds.Count);
    }

    [Fact]
    public async Task PersistNewClientWithContractsAsync_EmptyContracts_ShouldReturnValidationFailure()
    {
        var service = new EfCoreBrioPersistenceService(_contextFactory, NullLogger<EfCoreBrioPersistenceService>.Instance);
        var client = new Client(Guid.NewGuid(), "Sophie", "Dubois", new DateOnly(1988, 2, 14), Language.French, null);

        var result = await service.PersistNewClientWithContractsAsync(client, Array.Empty<ContratVie>());

        Assert.Equal(BrioPersistenceOutcome.ValidationFailure, result.Outcome);
        Assert.Single(result.Issues);
        Assert.Equal(BrioPersistenceSeverity.Error, result.Issues.First().Severity);
    }

    [Fact]
    public async Task PersistContractsForExistingClientAsync_ClientNotFound_ShouldReturnClientNotFound()
    {
        var service = new EfCoreBrioPersistenceService(_contextFactory, NullLogger<EfCoreBrioPersistenceService>.Instance);
        var unknownClientId = Guid.NewGuid();
        var contract = new ContratVie(Guid.NewGuid(), new ContractNumber("POL-999"), ContractType.IndividualLifeInsurance, ContractStatus.Active, unknownClientId, null);
        contract.AddExternalReference(new ExternalReference(SourceSystem.Brio, ReferenceType.PolicyNumber, "REF-999"));

        var result = await service.PersistContractsForExistingClientAsync(unknownClientId, new[] { contract });

        Assert.Equal(BrioPersistenceOutcome.ClientNotFound, result.Outcome);
    }

    [Fact]
    public async Task PersistNewClientWithContractsAsync_DuplicateInternalReference_ShouldReturnValidationFailure()
    {
        var service = new EfCoreBrioPersistenceService(_contextFactory, NullLogger<EfCoreBrioPersistenceService>.Instance);
        var client = new Client(Guid.NewGuid(), "Luc", "Bernard", new DateOnly(1970, 8, 25), Language.French, null);

        var contract1 = new ContratVie(Guid.NewGuid(), new ContractNumber("POL-DUP1"), ContractType.PLCI, ContractStatus.Active, client.Id, null);
        contract1.AddExternalReference(new ExternalReference(SourceSystem.Brio, ReferenceType.PolicyNumber, "DUP-REF"));

        var contract2 = new ContratVie(Guid.NewGuid(), new ContractNumber("POL-DUP2"), ContractType.EIP, ContractStatus.Active, client.Id, null);
        contract2.AddExternalReference(new ExternalReference(SourceSystem.Brio, ReferenceType.PolicyNumber, "DUP-REF"));

        var result = await service.PersistNewClientWithContractsAsync(client, new[] { contract1, contract2 });

        Assert.Equal(BrioPersistenceOutcome.ValidationFailure, result.Outcome);
        Assert.Contains("doublons", result.Issues.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task PersistNewClientWithContractsAsync_ExistingReference_ShouldRollbackAndReturnDuplicateExternalReference()
    {
        var service = new EfCoreBrioPersistenceService(_contextFactory, NullLogger<EfCoreBrioPersistenceService>.Instance);

        // Créer un client avec un contrat existant
        var clientId = Guid.NewGuid();
        var client = new Client(clientId, "Alice", "Existing", new DateOnly(1980, 1, 1), Language.French, null);
        var existingContract = new ContratVie(Guid.NewGuid(), new ContractNumber("POL-EXIST"), ContractType.IndividualLifeInsurance, ContractStatus.Active, clientId, null);
        existingContract.AddExternalReference(new ExternalReference(SourceSystem.Brio, ReferenceType.PolicyNumber, "CONFLICT-REF"));
        await service.PersistNewClientWithContractsAsync(client, new[] { existingContract });

        // Essayer d'ajouter un nouveau contrat avec la même référence pour le même client
        var conflictingContract = new ContratVie(Guid.NewGuid(), new ContractNumber("POL-CONFLICT"), ContractType.PLCI, ContractStatus.Active, clientId, null);
        conflictingContract.AddExternalReference(new ExternalReference(SourceSystem.Brio, ReferenceType.PolicyNumber, "CONFLICT-REF"));

        var result = await service.PersistContractsForExistingClientAsync(clientId, new[] { conflictingContract });

        Assert.Equal(BrioPersistenceOutcome.DuplicateExternalReference, result.Outcome);

        // Vérifier que le rollback a fonctionné : le contrat conflictuel ne doit pas être persisté
        await using (var context = await _contextFactory.CreateDbContextAsync())
        {
            var conflictExists = await context.Contracts.AnyAsync(c => c.Id == conflictingContract.Id);
            Assert.False(conflictExists);
        }
    }

    [Fact]
    public async Task PersistNewClientWithContractsAsync_Cancellation_ShouldPropagateOperationCanceledException()
    {
        var service = new EfCoreBrioPersistenceService(_contextFactory, NullLogger<EfCoreBrioPersistenceService>.Instance);
        var client = new Client(Guid.NewGuid(), "CancelTest", "Client", new DateOnly(1990, 1, 1), Language.French, null);
        var contract = new ContratVie(Guid.NewGuid(), new ContractNumber("POL-CANCEL"), ContractType.IndividualLifeInsurance, ContractStatus.Active, client.Id, null);
        contract.AddExternalReference(new ExternalReference(SourceSystem.Brio, ReferenceType.PolicyNumber, "REF-CANCEL"));

        var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            await service.PersistNewClientWithContractsAsync(client, new[] { contract }, cts.Token));
    }

    [Fact]
    public async Task PersistNewClientWithContractsAsync_ConcurrentWrites_OnlyOneSucceeds()
    {
        var service = new EfCoreBrioPersistenceService(_contextFactory, NullLogger<EfCoreBrioPersistenceService>.Instance);

        // Créer un client partagé avec un contrat initial
        var sharedClientId = Guid.NewGuid();
        var client = new Client(sharedClientId, "Concurrent", "Test", new DateOnly(1980, 1, 1), Language.French, null);
        var initialContract = new ContratVie(Guid.NewGuid(), new ContractNumber("POL-INIT-CONC"), ContractType.EIP, ContractStatus.Active, sharedClientId, null);
        initialContract.AddExternalReference(new ExternalReference(SourceSystem.Brio, ReferenceType.PolicyNumber, "INIT-CONC-REF"));
        await service.PersistNewClientWithContractsAsync(client, new[] { initialContract });

        // Deux tentatives concurrentes d'ajout de contrats avec la même référence pour le même client
        var contract1 = new ContratVie(Guid.NewGuid(), new ContractNumber("POL-CONC1"), ContractType.IndividualLifeInsurance, ContractStatus.Active, sharedClientId, null);
        contract1.AddExternalReference(new ExternalReference(SourceSystem.Brio, ReferenceType.PolicyNumber, "CONCURRENT-REF"));

        var contract2 = new ContratVie(Guid.NewGuid(), new ContractNumber("POL-CONC2"), ContractType.PLCI, ContractStatus.Active, sharedClientId, null);
        contract2.AddExternalReference(new ExternalReference(SourceSystem.Brio, ReferenceType.PolicyNumber, "CONCURRENT-REF"));

        var task1 = service.PersistContractsForExistingClientAsync(sharedClientId, new[] { contract1 });
        var task2 = service.PersistContractsForExistingClientAsync(sharedClientId, new[] { contract2 });

        var results = await Task.WhenAll(task1, task2);

        var successCount = results.Count(r => r.Outcome == BrioPersistenceOutcome.Success);
        var duplicateCount = results.Count(r => r.Outcome == BrioPersistenceOutcome.DuplicateExternalReference);

        Assert.Equal(1, successCount);
        Assert.Equal(1, duplicateCount);

        // Vérifier qu'exactement un seul contrat existe en base
        await using (var context = await _contextFactory.CreateDbContextAsync())
        {
            var persistedCount = await context.Contracts
                .Where(c => c.ExternalReferences.Any(r => r.Value == "CONCURRENT-REF"))
                .CountAsync();
            Assert.Equal(1, persistedCount);
        }
    }

    [Fact]
    public async Task PersistNewClientWithContractsAsync_DifferentClientsSamePolicyNumber_ShouldSucceed()
    {
        var service = new EfCoreBrioPersistenceService(_contextFactory, NullLogger<EfCoreBrioPersistenceService>.Instance);

        var client1 = new Client(Guid.NewGuid(), "Client1", "Test", new DateOnly(1980, 1, 1), Language.French, null);
        var contract1 = new ContratVie(Guid.NewGuid(), new ContractNumber("SAME-POL-NUM"), ContractType.IndividualLifeInsurance, ContractStatus.Active, client1.Id, null);
        contract1.AddExternalReference(new ExternalReference(SourceSystem.Brio, ReferenceType.PolicyNumber, "REF-CLIENT1"));

        var client2 = new Client(Guid.NewGuid(), "Client2", "Test", new DateOnly(1990, 1, 1), Language.French, null);
        var contract2 = new ContratVie(Guid.NewGuid(), new ContractNumber("SAME-POL-NUM"), ContractType.PLCI, ContractStatus.Active, client2.Id, null);
        contract2.AddExternalReference(new ExternalReference(SourceSystem.Brio, ReferenceType.PolicyNumber, "REF-CLIENT2"));

        var result1 = await service.PersistNewClientWithContractsAsync(client1, new[] { contract1 });
        var result2 = await service.PersistNewClientWithContractsAsync(client2, new[] { contract2 });

        Assert.Equal(BrioPersistenceOutcome.Success, result1.Outcome);
        Assert.Equal(BrioPersistenceOutcome.Success, result2.Outcome);
    }

    [Fact]
    public async Task PersistNewClientWithContractsAsync_VisibleInNewContext()
    {
        var service = new EfCoreBrioPersistenceService(_contextFactory, NullLogger<EfCoreBrioPersistenceService>.Instance);
        var client = new Client(Guid.NewGuid(), "NewContext", "Test", new DateOnly(1985, 5, 20), Language.French, null);
        var contract = new ContratVie(Guid.NewGuid(), new ContractNumber("POL-NEWCTX"), ContractType.IndividualLifeInsurance, ContractStatus.Active, client.Id, null);
        contract.AddExternalReference(new ExternalReference(SourceSystem.Brio, ReferenceType.PolicyNumber, "REF-NEWCTX"));

        await service.PersistNewClientWithContractsAsync(client, new[] { contract });

        await using var context = await _contextFactory.CreateDbContextAsync();
        var persistedClient = await context.Clients.FirstOrDefaultAsync(c => c.Id == client.Id);
        var persistedContract = await context.Contracts.FirstOrDefaultAsync(c => c.Id == contract.Id);

        Assert.NotNull(persistedClient);
        Assert.NotNull(persistedContract);
    }

    private sealed class TestDbContextFactory : IDbContextFactory<AppDbContext>
    {
        private readonly DbContextOptions<AppDbContext> _options;

        public TestDbContextFactory(DbContextOptions<AppDbContext> options)
        {
            _options = options;
        }

        public AppDbContext CreateDbContext() => new(_options);
    }
}
