using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Praxis360.Domain.Types;
using Praxis360_v1.Domain.Aggregates;
using Praxis360_v1.Domain.Entities;
using Praxis360_v1.Domain.ValueObjects;
using Praxis360_v1.Infrastructure.Persistence;
using Praxis360_v1.Infrastructure.Repositories;

namespace Praxis360_v1.Tests.Infrastructure.Repositories;

public sealed class EfCoreContractRepositoryTests : IDisposable
{
    private readonly string _tempDbPath;
    private readonly SqliteConnection _keepAliveConnection;
    private readonly IDbContextFactory<AppDbContext> _contextFactory;
    private readonly Guid _testClientId;

    public EfCoreContractRepositoryTests()
    {
        _testClientId = Guid.NewGuid();
        _tempDbPath = Path.Combine(Path.GetTempPath(), $"Praxis360_EfCoreContractRepo_{Guid.NewGuid():N}.db");
        var connectionString = $"Data Source={_tempDbPath};Foreign Keys=True";

        _keepAliveConnection = new SqliteConnection(connectionString);
        _keepAliveConnection.Open();

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_keepAliveConnection);

        _contextFactory = new TestDbContextFactory(optionsBuilder.Options);

        using var context = _contextFactory.CreateDbContext();
        context.Database.Migrate();

        // Créer un client de test
        var testClient = new Client(_testClientId, "Test", "Client", new DateOnly(1980, 1, 1), Language.French, null);
        var clientRepo = new EfCoreClientRepository(_contextFactory);
        clientRepo.SaveAsync(testClient).Wait();
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
    public async Task SaveAsync_ThenGetByIdAsync_ShouldReturnContract()
    {
        var repository = new EfCoreContractRepository(_contextFactory);
        var contract = new ContratVie(
            id: Guid.NewGuid(),
            number: new ContractNumber("POL-001"),
            type: ContractType.PLCI,
            status: ContractStatus.Active,
            clientId: _testClientId,
            insurer: null
        );

        var reference = new ExternalReference(SourceSystem.Brio, ReferenceType.PolicyNumber, "REF-001");
        contract.AddExternalReference(reference);

        var provenance = new ContractProvenance(
            SourceSystem.Brio,
            DateTime.UtcNow,
            "Test Insurer",
            new DateOnly(2025, 1, 27)
        );
        contract.AddProvenance(provenance);

        await repository.SaveAsync(contract);

        var retrieved = await repository.GetByIdAsync(contract.Id);

        Assert.NotNull(retrieved);
        Assert.Equal(contract.Id, retrieved.Id);
        Assert.Equal("POL-001", retrieved.Number.Value);
        Assert.Single(retrieved.ExternalReferences);
        Assert.Single(retrieved.Provenances);
    }

    [Fact]
    public async Task GetByIdAsync_NotFound_ShouldReturnNull()
    {
        var repository = new EfCoreContractRepository(_contextFactory);
        var retrieved = await repository.GetByIdAsync(Guid.NewGuid());
        Assert.Null(retrieved);
    }

    [Fact]
    public async Task GetByClientIdAsync_ShouldReturnAllClientContracts()
    {
        var repository = new EfCoreContractRepository(_contextFactory);
        var contract1 = new ContratVie(Guid.NewGuid(), new ContractNumber("POL-100"), ContractType.PLCI, ContractStatus.Active, _testClientId, null);
        var contract2 = new ContratVie(Guid.NewGuid(), new ContractNumber("POL-200"), ContractType.EIP, ContractStatus.PaidUp, _testClientId, null);

        await repository.SaveAsync(contract1);
        await repository.SaveAsync(contract2);

        var results = await repository.GetByClientIdAsync(_testClientId);

        Assert.Equal(2, results.Count);
        Assert.Contains(results, c => c.Id == contract1.Id);
        Assert.Contains(results, c => c.Id == contract2.Id);
    }

    [Fact]
    public async Task FindByExternalReferenceAsync_ShouldReturnContract()
    {
        var repository = new EfCoreContractRepository(_contextFactory);
        var contract = new ContratVie(Guid.NewGuid(), new ContractNumber("POL-300"), ContractType.IndividualLifeInsurance, ContractStatus.Active, _testClientId, null);
        var reference = new ExternalReference(SourceSystem.Brio, ReferenceType.PolicyNumber, "UNIQUE-REF-999");
        contract.AddExternalReference(reference);

        await repository.SaveAsync(contract);

        var found = await repository.FindByExternalReferenceAsync(_testClientId, SourceSystem.Brio, ReferenceType.PolicyNumber, "UNIQUE-REF-999");

        Assert.NotNull(found);
        Assert.Equal(contract.Id, found.Id);
    }

    [Fact]
    public async Task FindByExternalReferenceAsync_DifferentClientId_ShouldReturnNull()
    {
        var repository = new EfCoreContractRepository(_contextFactory);
        var contract = new ContratVie(Guid.NewGuid(), new ContractNumber("POL-400"), ContractType.IndividualLifeInsurance, ContractStatus.Active, _testClientId, null);
        var reference = new ExternalReference(SourceSystem.Brio, ReferenceType.PolicyNumber, "REF-CLIENT-SPECIFIC");
        contract.AddExternalReference(reference);

        await repository.SaveAsync(contract);

        var found = await repository.FindByExternalReferenceAsync(Guid.NewGuid(), SourceSystem.Brio, ReferenceType.PolicyNumber, "REF-CLIENT-SPECIFIC");

        Assert.Null(found);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldLoadReferencesAndProvenances()
    {
        var repository = new EfCoreContractRepository(_contextFactory);
        var contract = new ContratVie(Guid.NewGuid(), new ContractNumber("POL-500"), ContractType.PLCI, ContractStatus.Active, _testClientId, null);

        contract.AddExternalReference(new ExternalReference(SourceSystem.Brio, ReferenceType.PolicyNumber, "REF-A"));
        contract.AddExternalReference(new ExternalReference(SourceSystem.Brio, ReferenceType.ContractNumber, "REF-B"));

        contract.AddProvenance(new ContractProvenance(SourceSystem.Brio, DateTime.UtcNow, "Insurer1", new DateOnly(2025, 1, 1)));
        contract.AddProvenance(new ContractProvenance(SourceSystem.Brio, DateTime.UtcNow.AddDays(1), "Insurer1", new DateOnly(2025, 1, 2)));

        await repository.SaveAsync(contract);

        var retrieved = await repository.GetByIdAsync(contract.Id);

        Assert.NotNull(retrieved);
        Assert.Equal(2, retrieved.ExternalReferences.Count);
        Assert.Equal(2, retrieved.Provenances.Count);
    }

    [Fact]
    public async Task UpdateAsync_ShouldPersistChanges()
    {
        var repository = new EfCoreContractRepository(_contextFactory);
        var contract = new ContratVie(Guid.NewGuid(), new ContractNumber("POL-600"), ContractType.PLCI, ContractStatus.Active, _testClientId, null);
        await repository.SaveAsync(contract);

        var updated = new ContratVie(contract.Id, new ContractNumber("POL-600"), ContractType.PLCI, ContractStatus.Terminated, _testClientId, null);
        await repository.UpdateAsync(updated);

        var retrieved = await repository.GetByIdAsync(contract.Id);
        Assert.NotNull(retrieved);
        Assert.Equal(ContractStatus.Terminated, retrieved.Status);
    }

    [Fact]
    public async Task SaveAsync_VisibleInNewContext()
    {
        var repository = new EfCoreContractRepository(_contextFactory);
        var contract = new ContratVie(Guid.NewGuid(), new ContractNumber("POL-700"), ContractType.IndividualLifeInsurance, ContractStatus.Active, _testClientId, null);
        contract.AddExternalReference(new ExternalReference(SourceSystem.Brio, ReferenceType.PolicyNumber, "NEW-CTX-REF"));
        await repository.SaveAsync(contract);

        var newRepository = new EfCoreContractRepository(_contextFactory);
        var retrieved = await newRepository.GetByIdAsync(contract.Id);

        Assert.NotNull(retrieved);
        Assert.Equal("POL-700", retrieved.Number.Value);
        Assert.Single(retrieved.ExternalReferences);
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
