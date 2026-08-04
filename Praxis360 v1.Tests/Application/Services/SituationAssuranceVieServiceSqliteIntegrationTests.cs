using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Praxis360.Domain.Types;
using Praxis360_v1.Application.Models;
using Praxis360_v1.Application.Services;
using Praxis360_v1.Domain.Types;
using Praxis360_v1.Infrastructure.FileReaders;
using Praxis360_v1.Infrastructure.Persistence;
using Praxis360_v1.Infrastructure.Repositories;
using Praxis360_v1.Infrastructure.Services;
using Praxis360_v1.Services;
using Praxis360_v1.Tests.TestSupport;

namespace Praxis360_v1.Tests.Application.Services;

/// <summary>
/// Tests d'intégration SQLite pour SituationAssuranceVieService.
/// Vérifie le rechargement de situation depuis SQLite après un import BRIO réussi.
/// </summary>
public sealed class SituationAssuranceVieServiceSqliteIntegrationTests : IAsyncDisposable
{
    private readonly string _tempDbPath;
    private readonly SqliteConnection _keepAliveConnection;
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public SituationAssuranceVieServiceSqliteIntegrationTests()
    {
        _tempDbPath = Path.Combine(Path.GetTempPath(), $"Praxis360_SitAV_{Guid.NewGuid():N}.db");
        var connectionString = $"Data Source={_tempDbPath};Foreign Keys=True";

        _keepAliveConnection = new SqliteConnection(connectionString);
        _keepAliveConnection.Open();

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_keepAliveConnection);

        _contextFactory = new TestDbContextFactory(optionsBuilder.Options);

        using var context = _contextFactory.CreateDbContext();
        context.Database.Migrate();
    }

    public async ValueTask DisposeAsync()
    {
        if (_contextFactory is IAsyncDisposable asyncDisposableFactory)
        {
            await asyncDisposableFactory.DisposeAsync();
        }
        else if (_contextFactory is IDisposable disposableFactory)
        {
            disposableFactory.Dispose();
        }

        await _keepAliveConnection.DisposeAsync();

        if (File.Exists(_tempDbPath))
        {
            try { File.Delete(_tempDbPath); } catch { /* Test cleanup */ }
        }
    }

    [Fact]
    public async Task EndToEnd_BrioImportAndReloadFromSqlite_ShouldConstructAccurateSituationReadModel()
    {
        // Arrange: Parse BRIO fixture and apply
        var reader = new BrioCsvFileReader();
        var analyzer = new BrioImportAnalyzer();
        var clientRepository = new EfCoreClientRepository(_contextFactory);
        var contractRepository = new EfCoreContractRepository(_contextFactory);
        var persistenceService = new EfCoreBrioPersistenceService(_contextFactory, NullLogger<EfCoreBrioPersistenceService>.Instance);
        var applicationService = new BrioContractApplicationService(clientRepository, contractRepository, persistenceService);

        using var stream = BrioSyntheticFixtureLoader.LoadFixture("BrioSynthetic.ValidCore.csv");
        var readResult = await reader.ReadAsync(stream);
        var analysisResult = await analyzer.AnalyzeAsync(readResult);

        var alphaIdentity = analysisResult.ClientCandidates.Values
            .First(c => c.NormalizedIdentity.StartsWith("INAMI:"))
            .NormalizedIdentity;

        var applicationResult = await applicationService.ApplyWithNewClientAsync(
            analysisResult,
            alphaIdentity,
            Language.French);

        Assert.Equal(ApplicationOutcome.Success, applicationResult.Outcome);
        Assert.NotNull(applicationResult.ClientId);
        Assert.True(applicationResult.ClientWasCreated);

        Guid persistedClientId = applicationResult.ClientId.Value;

        // Act: Reload situation from SQLite using SituationAssuranceVieService (new instances)
        var situationClientRepository = new EfCoreClientRepository(_contextFactory);
        var situationContractRepository = new EfCoreContractRepository(_contextFactory);
        var situationService = new SituationAssuranceVieService(situationClientRepository, situationContractRepository);

        var situation = await situationService.GetSituationForClientAsync(persistedClientId);

        // Assert: Situation was loaded
        Assert.NotNull(situation);
        Assert.Equal(persistedClientId, situation.ClientId);
        Assert.False(string.IsNullOrWhiteSpace(situation.ClientDisplayName));
        Assert.Equal("DR. ALPHA SYNTHETIC ALPHA", situation.ClientDisplayName);

        // Assert: Contract counts (ALPHA has 2 contracts both active in fixture)
        Assert.Equal(2, situation.TotalContracts);
        Assert.Equal(2, situation.CurrentContracts);

        // Assert: Financial indicators remain absent
        Assert.Null(situation.ReserveAcquise);
        Assert.Null(situation.CapitalATerme);
        Assert.Null(situation.CapitalDeces);
        Assert.Null(situation.RevenuGaranti);

        // Assert: Contract details (ALPHA has SYN-ALPHA-001 PLCI and SYN-ALPHA-002 EIP)
        Assert.Equal(2, situation.Contracts.Count);
        var contractNumbers = situation.Contracts.Select(c => c.Number).OrderBy(n => n).ToList();
        Assert.Contains("SYN-ALPHA-001", contractNumbers);
        Assert.Contains("SYN-ALPHA-002", contractNumbers);

        var contractTypes = situation.Contracts.Select(c => c.ContractType).Distinct().ToList();
        Assert.Contains(ContractType.PLCI, contractTypes);
        Assert.Contains(ContractType.EIP, contractTypes);

        var statuses = situation.Contracts.Select(c => c.Status).Distinct().ToList();
        Assert.All(statuses, s => Assert.Equal(ContractStatus.Active, s));

        // Assert: No duplicates
        var distinctNumbers = situation.Contracts.Select(c => c.Number).Distinct().Count();
        Assert.Equal(situation.Contracts.Count, distinctNumbers);

        // Assert: Insurer fallback logic (fixture has no insurer data)
        foreach (var contract in situation.Contracts)
        {
            Assert.NotNull(contract.InsurerDisplayName);
            Assert.Equal("Compagnie non disponible", contract.InsurerDisplayName);
        }
    }

    [Fact]
    public async Task GetSituationForDefaultClientAsync_WhenNoClients_ShouldReturnNoClientsAvailable()
    {
        // Arrange: Empty database
        var clientRepository = new EfCoreClientRepository(_contextFactory);
        var contractRepository = new EfCoreContractRepository(_contextFactory);
        var situationService = new SituationAssuranceVieService(clientRepository, contractRepository);

        // Act
        var result = await situationService.GetSituationForDefaultClientAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(Praxis360_v1.Models.SituationAssuranceVieLoadStatus.NoClientsAvailable, result.Status);
        Assert.Null(result.Situation);
    }

    [Fact]
    public async Task GetSituationForDefaultClientAsync_WhenOneClient_ShouldReturnClientLoaded()
    {
        // Arrange: Import one client
        var reader = new BrioCsvFileReader();
        var analyzer = new BrioImportAnalyzer();
        var clientRepository = new EfCoreClientRepository(_contextFactory);
        var contractRepository = new EfCoreContractRepository(_contextFactory);
        var persistenceService = new EfCoreBrioPersistenceService(_contextFactory, NullLogger<EfCoreBrioPersistenceService>.Instance);
        var applicationService = new BrioContractApplicationService(clientRepository, contractRepository, persistenceService);

        using var stream = BrioSyntheticFixtureLoader.LoadFixture("BrioSynthetic.ValidCore.csv");
        var readResult = await reader.ReadAsync(stream);
        var analysisResult = await analyzer.AnalyzeAsync(readResult);

        var alphaIdentity = analysisResult.ClientCandidates.Values
            .First(c => c.NormalizedIdentity.StartsWith("INAMI:"))
            .NormalizedIdentity;

        await applicationService.ApplyWithNewClientAsync(analysisResult, alphaIdentity, Language.French);

        // Act: Create new instances to simulate reload
        var situationClientRepository = new EfCoreClientRepository(_contextFactory);
        var situationContractRepository = new EfCoreContractRepository(_contextFactory);
        var situationService = new SituationAssuranceVieService(situationClientRepository, situationContractRepository);

        var result = await situationService.GetSituationForDefaultClientAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(Praxis360_v1.Models.SituationAssuranceVieLoadStatus.ClientLoaded, result.Status);
        Assert.NotNull(result.Situation);
        Assert.Equal("DR. ALPHA SYNTHETIC ALPHA", result.Situation.ClientDisplayName);
    }

    [Fact]
    public async Task GetSituationForDefaultClientAsync_WhenMultipleClients_ShouldReturnMultipleClientsRequireSelection()
    {
        // Arrange: Import two distinct clients
        var reader = new BrioCsvFileReader();
        var analyzer = new BrioImportAnalyzer();
        var clientRepository = new EfCoreClientRepository(_contextFactory);
        var contractRepository = new EfCoreContractRepository(_contextFactory);
        var persistenceService = new EfCoreBrioPersistenceService(_contextFactory, NullLogger<EfCoreBrioPersistenceService>.Instance);
        var applicationService = new BrioContractApplicationService(clientRepository, contractRepository, persistenceService);

        using var stream = BrioSyntheticFixtureLoader.LoadFixture("BrioSynthetic.ValidCore.csv");
        var readResult = await reader.ReadAsync(stream);
        var analysisResult = await analyzer.AnalyzeAsync(readResult);

        var alphaIdentity = analysisResult.ClientCandidates.Values
            .First(c => c.NormalizedIdentity.StartsWith("INAMI:"))
            .NormalizedIdentity;

        var betaIdentity = analysisResult.ClientCandidates.Values
            .First(c => c.NormalizedIdentity.Contains("1980-06-22"))
            .NormalizedIdentity;

        await applicationService.ApplyWithNewClientAsync(analysisResult, alphaIdentity, Language.French);
        await applicationService.ApplyWithNewClientAsync(analysisResult, betaIdentity, Language.French);

        // Act: Create new instances to simulate reload
        var situationClientRepository = new EfCoreClientRepository(_contextFactory);
        var situationContractRepository = new EfCoreContractRepository(_contextFactory);
        var situationService = new SituationAssuranceVieService(situationClientRepository, situationContractRepository);

        var result = await situationService.GetSituationForDefaultClientAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(Praxis360_v1.Models.SituationAssuranceVieLoadStatus.MultipleClientsRequireSelection, result.Status);
        Assert.Null(result.Situation);
    }

    [Fact]
    public async Task GetSituationForClientAsync_WhenClientDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var clientRepository = new EfCoreClientRepository(_contextFactory);
        var contractRepository = new EfCoreContractRepository(_contextFactory);
        var situationService = new SituationAssuranceVieService(clientRepository, contractRepository);
        var nonExistentClientId = Guid.NewGuid();

        // Act
        var situation = await situationService.GetSituationForClientAsync(nonExistentClientId);

        // Assert
        Assert.Null(situation);
    }

    [Fact]
    public async Task GetSituationForClientAsync_WhenClientHasNoContracts_ShouldReturnSituationWithZeroContracts()
    {
        // Arrange: Create a client manually with no contracts
        var clientRepository = new EfCoreClientRepository(_contextFactory);
        var client = new Praxis360_v1.Domain.Entities.Client(
            id: Guid.NewGuid(),
            firstName: "Empty",
            lastName: "Client",
            dateOfBirth: new DateOnly(1980, 1, 1),
            preferredLanguage: Language.French
        );
        await clientRepository.SaveAsync(client);

        // Act: Create new instances
        var situationClientRepository = new EfCoreClientRepository(_contextFactory);
        var contractRepository = new EfCoreContractRepository(_contextFactory);
        var situationService = new SituationAssuranceVieService(situationClientRepository, contractRepository);

        var situation = await situationService.GetSituationForClientAsync(client.Id);

        // Assert
        Assert.NotNull(situation);
        Assert.Equal(client.Id, situation.ClientId);
        Assert.Equal(0, situation.TotalContracts);
        Assert.Equal(0, situation.CurrentContracts);
        Assert.Empty(situation.Contracts);
    }

    [Fact]
    public async Task CurrentContracts_ShouldCountOnlyActiveOrPaidUpOrSuspended()
    {
        // Arrange: Import fixture with known statuses
        var reader = new BrioCsvFileReader();
        var analyzer = new BrioImportAnalyzer();
        var clientRepository = new EfCoreClientRepository(_contextFactory);
        var contractRepository = new EfCoreContractRepository(_contextFactory);
        var persistenceService = new EfCoreBrioPersistenceService(_contextFactory, NullLogger<EfCoreBrioPersistenceService>.Instance);
        var applicationService = new BrioContractApplicationService(clientRepository, contractRepository, persistenceService);

        using var stream = BrioSyntheticFixtureLoader.LoadFixture("BrioSynthetic.ValidCore.csv");
        var readResult = await reader.ReadAsync(stream);
        var analysisResult = await analyzer.AnalyzeAsync(readResult);

        var alphaIdentity = analysisResult.ClientCandidates.Values
            .First(c => c.NormalizedIdentity.StartsWith("INAMI:"))
            .NormalizedIdentity;

        await applicationService.ApplyWithNewClientAsync(analysisResult, alphaIdentity, Language.French);

        // Act: Create new instances
        var situationClientRepository = new EfCoreClientRepository(_contextFactory);
        var situationContractRepository = new EfCoreContractRepository(_contextFactory);
        var situationService = new SituationAssuranceVieService(situationClientRepository, situationContractRepository);

        var result = await situationService.GetSituationForDefaultClientAsync();

        // Assert
        Assert.NotNull(result.Situation);
        var activeCount = result.Situation.Contracts.Count(c =>
            c.Status == ContractStatus.Active ||
            c.Status == ContractStatus.PaidUp ||
            c.Status == ContractStatus.Suspended
        );

        Assert.Equal(activeCount, result.Situation.CurrentContracts);
    }

    [Fact]
    public async Task InsurerFallback_WhenBrioProvenanceHasNoInsurerName_ShouldReturnCompagnieNonDisponible()
    {
        // Arrange: Import known fixture (this fixture has no ProductCompanyLabel values)
        var reader = new BrioCsvFileReader();
        var analyzer = new BrioImportAnalyzer();
        var clientRepository = new EfCoreClientRepository(_contextFactory);
        var contractRepository = new EfCoreContractRepository(_contextFactory);
        var persistenceService = new EfCoreBrioPersistenceService(_contextFactory, NullLogger<EfCoreBrioPersistenceService>.Instance);
        var applicationService = new BrioContractApplicationService(clientRepository, contractRepository, persistenceService);

        using var stream = BrioSyntheticFixtureLoader.LoadFixture("BrioSynthetic.ValidCore.csv");
        var readResult = await reader.ReadAsync(stream);
        var analysisResult = await analyzer.AnalyzeAsync(readResult);

        var alphaIdentity = analysisResult.ClientCandidates.Values
            .First(c => c.NormalizedIdentity.StartsWith("INAMI:"))
            .NormalizedIdentity;

        await applicationService.ApplyWithNewClientAsync(analysisResult, alphaIdentity, Language.French);

        // Act: Create new instances
        var situationClientRepository = new EfCoreClientRepository(_contextFactory);
        var situationContractRepository = new EfCoreContractRepository(_contextFactory);
        var situationService = new SituationAssuranceVieService(situationClientRepository, situationContractRepository);

        var result = await situationService.GetSituationForDefaultClientAsync();

        // Assert: Since the fixture has no insurer data, fallback should be "Compagnie non disponible"
        Assert.NotNull(result.Situation);
        foreach (var contract in result.Situation.Contracts)
        {
            Assert.NotNull(contract.InsurerDisplayName);
            Assert.Equal("Compagnie non disponible", contract.InsurerDisplayName);
        }
    }

    [Fact]
    public async Task InsurerFallback_WhenInsurerIsNullButBrioProvenanceHasName_ShouldUseMostRecentBrioProvenance()
    {
        // Arrange: Create a client and contract manually with synthetic BRIO provenances
        var clientRepository = new EfCoreClientRepository(_contextFactory);
        var contractRepository = new EfCoreContractRepository(_contextFactory);

        var client = new Praxis360_v1.Domain.Entities.Client(
            id: Guid.NewGuid(),
            firstName: "Synthetic",
            lastName: "Fallback",
            dateOfBirth: new DateOnly(1975, 5, 15),
            preferredLanguage: Language.French
        );
        await clientRepository.SaveAsync(client);

        var contract = new Praxis360_v1.Domain.Aggregates.ContratVie(
            id: Guid.NewGuid(),
            number: new Praxis360_v1.Domain.ValueObjects.ContractNumber("SYNTH-FALLBACK-001"),
            type: ContractType.PLCI,
            status: ContractStatus.Active,
            clientId: client.Id,
            insurer: null // Insurer is explicitly null to force fallback
        );

        // Add two BRIO provenances: older and newer
        var olderProvenance = new Praxis360_v1.Domain.ValueObjects.ContractProvenance(
            sourceSystem: SourceSystem.Brio,
            importedAtUtc: DateTime.SpecifyKind(new DateTime(2025, 1, 1, 10, 0, 0), DateTimeKind.Utc),
            rawInsurerName: "Older Synthetic Insurance Co."
        );

        var newerProvenance = new Praxis360_v1.Domain.ValueObjects.ContractProvenance(
            sourceSystem: SourceSystem.Brio,
            importedAtUtc: DateTime.SpecifyKind(new DateTime(2025, 1, 15, 14, 30, 0), DateTimeKind.Utc),
            rawInsurerName: "Newer Synthetic Insurance Co."
        );

        contract.AddProvenance(olderProvenance);
        contract.AddProvenance(newerProvenance);

        await contractRepository.SaveAsync(contract);

        // Act: Create new instances and load situation
        var situationClientRepository = new EfCoreClientRepository(_contextFactory);
        var situationContractRepository = new EfCoreContractRepository(_contextFactory);
        var situationService = new SituationAssuranceVieService(situationClientRepository, situationContractRepository);

        var situation = await situationService.GetSituationForClientAsync(client.Id);

        // Assert: Should use the most recent BRIO provenance's RawInsurerName
        Assert.NotNull(situation);
        Assert.Single(situation.Contracts);
        var contractReadModel = situation.Contracts[0];
        Assert.NotNull(contractReadModel.InsurerDisplayName);
        Assert.Equal("Newer Synthetic Insurance Co.", contractReadModel.InsurerDisplayName);
    }

    private sealed class TestDbContextFactory : IDbContextFactory<AppDbContext>
    {
        private readonly DbContextOptions<AppDbContext> _options;

        public TestDbContextFactory(DbContextOptions<AppDbContext> options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public AppDbContext CreateDbContext()
        {
            return new AppDbContext(_options);
        }

        public async Task<AppDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(CreateDbContext());
        }
    }
}
