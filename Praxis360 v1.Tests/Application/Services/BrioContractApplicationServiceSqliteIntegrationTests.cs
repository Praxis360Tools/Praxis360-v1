using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Praxis360.Domain.Types;
using Praxis360_v1.Application.Interfaces;
using Praxis360_v1.Application.Models;
using Praxis360_v1.Application.Services;
using Praxis360_v1.Infrastructure.FileReaders;
using Praxis360_v1.Infrastructure.Persistence;
using Praxis360_v1.Infrastructure.Repositories;
using Praxis360_v1.Infrastructure.Services;
using Praxis360_v1.Tests.TestSupport;

namespace Praxis360_v1.Tests.Application.Services;

/// <summary>
/// Tests d'intégration SQLite pour BrioContractApplicationService.
/// Vérifie la chaîne complète BrioContractApplicationService → EfCoreBrioPersistenceService → AppDbContext → SQLite.
/// </summary>
public sealed class BrioContractApplicationServiceSqliteIntegrationTests : IAsyncDisposable
{
    private readonly string _tempDbPath;
    private readonly SqliteConnection _keepAliveConnection;
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public BrioContractApplicationServiceSqliteIntegrationTests()
    {
        _tempDbPath = Path.Combine(Path.GetTempPath(), $"Praxis360_BrioAppSvc_{Guid.NewGuid():N}.db");
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
        await _keepAliveConnection.DisposeAsync();
        SqliteConnection.ClearAllPools();
        GC.Collect();
        GC.WaitForPendingFinalizers();

        if (File.Exists(_tempDbPath))
        {
            try { File.Delete(_tempDbPath); } catch { }
        }
    }

    [Fact]
    public async Task ApplyWithNewClientAsync_ValidCore_ShouldPersistToSqliteAndBeVisibleInNewDbContext()
    {
        // Arrange: Créer les services avec la vraie infrastructure SQLite
        var reader = new BrioCsvFileReader();
        var analyzer = new BrioImportAnalyzer();
        var clientRepository = new EfCoreClientRepository(_contextFactory);
        var contractRepository = new EfCoreContractRepository(_contextFactory);
        var persistenceService = new EfCoreBrioPersistenceService(_contextFactory, Microsoft.Extensions.Logging.Abstractions.NullLogger<EfCoreBrioPersistenceService>.Instance);
        var applicationService = new BrioContractApplicationService(clientRepository, contractRepository, persistenceService);

        using var stream = BrioSyntheticFixtureLoader.LoadFixture("BrioSynthetic.ValidCore.csv");
        var readResult = await reader.ReadAsync(stream);
        var analysisResult = await analyzer.AnalyzeAsync(readResult);

        var alphaIdentity = analysisResult.ClientCandidates.Values
            .First(c => c.NormalizedIdentity.StartsWith("INAMI:"))
            .NormalizedIdentity;

        // Act: Appliquer l'import avec un nouveau client
        var result = await applicationService.ApplyWithNewClientAsync(
            analysisResult,
            alphaIdentity,
            Language.French);

        // Assert: Vérifier le résultat de l'application
        Assert.Equal(ApplicationOutcome.Success, result.Outcome);
        Assert.NotNull(result.ClientId);
        Assert.True(result.ClientWasCreated);
        Assert.Equal(2, result.ContractsCreated.Count);
        Assert.Empty(result.ContractsAlreadyExisting);
        Assert.Empty(result.ContractsSkipped);

        // Assert: Créer un nouveau DbContext et vérifier que les données sont bien persistées dans SQLite
        await using var newContext = await _contextFactory.CreateDbContextAsync();
        var persistedClient = await newContext.Clients.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == result.ClientId.Value);

        Assert.NotNull(persistedClient);
        Assert.Equal("DR. ALPHA", persistedClient.FirstName);
        Assert.Equal("SYNTHETIC ALPHA", persistedClient.LastName);

        var persistedContracts = await newContext.Contracts.AsNoTracking()
            .Include(c => c.ExternalReferences)
            .Where(c => c.ClientId == result.ClientId.Value)
            .ToListAsync();

        Assert.Equal(2, persistedContracts.Count);
        Assert.All(persistedContracts, contract =>
        {
            Assert.NotEqual(Guid.Empty, contract.Id);
            Assert.Equal(result.ClientId.Value, contract.ClientId);
            Assert.NotEmpty(contract.ExternalReferences);
        });
    }

    [Fact]
    public async Task ApplyWithNewClientAsync_ShouldPersistLanguageFrenchToSqlite()
    {
        // Arrange
        var reader = new BrioCsvFileReader();
        var analyzer = new BrioImportAnalyzer();
        var clientRepository = new EfCoreClientRepository(_contextFactory);
        var contractRepository = new EfCoreContractRepository(_contextFactory);
        var persistenceService = new EfCoreBrioPersistenceService(_contextFactory, Microsoft.Extensions.Logging.Abstractions.NullLogger<EfCoreBrioPersistenceService>.Instance);
        var applicationService = new BrioContractApplicationService(clientRepository, contractRepository, persistenceService);

        using var stream = BrioSyntheticFixtureLoader.LoadFixture("BrioSynthetic.ValidCore.csv");
        var readResult = await reader.ReadAsync(stream);
        var analysisResult = await analyzer.AnalyzeAsync(readResult);

        var alphaIdentity = analysisResult.ClientCandidates.Values
            .First(c => c.NormalizedIdentity.StartsWith("INAMI:"))
            .NormalizedIdentity;

        // Act: Apply import with Language.French explicitly
        var result = await applicationService.ApplyWithNewClientAsync(
            analysisResult,
            alphaIdentity,
            Language.French);

        // Assert: Application succeeded
        Assert.Equal(ApplicationOutcome.Success, result.Outcome);
        Assert.NotNull(result.ClientId);
        Assert.True(result.ClientWasCreated);

        // Assert: Reload client from SQLite in new DbContext and verify PreferredLanguage is French
        await using var newContext = await _contextFactory.CreateDbContextAsync();
        var persistedClient = await newContext.Clients.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == result.ClientId.Value);

        Assert.NotNull(persistedClient);
        Assert.Equal(Language.French, persistedClient.PreferredLanguage);

        // Assert: Verify contracts are still associated
        var persistedContracts = await newContext.Contracts.AsNoTracking()
            .Where(c => c.ClientId == result.ClientId.Value)
            .ToListAsync();

        Assert.Equal(2, persistedContracts.Count);
    }

    [Fact]
    public async Task ApplyToExistingClientAsync_ReapplyingSameData_ShouldReturnContractsAlreadyExistingAndNoDuplicatesInSqlite()
    {
        // Arrange: Première application pour créer un client et des contrats
        var reader = new BrioCsvFileReader();
        var analyzer = new BrioImportAnalyzer();
        var clientRepository = new EfCoreClientRepository(_contextFactory);
        var contractRepository = new EfCoreContractRepository(_contextFactory);
        var persistenceService = new EfCoreBrioPersistenceService(_contextFactory, Microsoft.Extensions.Logging.Abstractions.NullLogger<EfCoreBrioPersistenceService>.Instance);
        var applicationService1 = new BrioContractApplicationService(clientRepository, contractRepository, persistenceService);

        using var stream1 = BrioSyntheticFixtureLoader.LoadFixture("BrioSynthetic.ValidCore.csv");
        var readResult1 = await reader.ReadAsync(stream1);
        var analysisResult1 = await analyzer.AnalyzeAsync(readResult1);

        var alphaIdentity = analysisResult1.ClientCandidates.Values
            .First(c => c.NormalizedIdentity.StartsWith("INAMI:"))
            .NormalizedIdentity;

        var firstResult = await applicationService1.ApplyWithNewClientAsync(
            analysisResult1,
            alphaIdentity,
            Language.French);

        Assert.Equal(ApplicationOutcome.Success, firstResult.Outcome);
        var clientId = firstResult.ClientId!.Value;

        // Act: Recréer tous les services (simuler une nouvelle session) et réappliquer les mêmes données
        var clientRepository2 = new EfCoreClientRepository(_contextFactory);
        var contractRepository2 = new EfCoreContractRepository(_contextFactory);
        var persistenceService2 = new EfCoreBrioPersistenceService(_contextFactory, Microsoft.Extensions.Logging.Abstractions.NullLogger<EfCoreBrioPersistenceService>.Instance);
        var applicationService2 = new BrioContractApplicationService(clientRepository2, contractRepository2, persistenceService2);

        using var stream2 = BrioSyntheticFixtureLoader.LoadFixture("BrioSynthetic.ValidCore.csv");
        var readResult2 = await reader.ReadAsync(stream2);
        var analysisResult2 = await analyzer.AnalyzeAsync(readResult2);

        var secondResult = await applicationService2.ApplyToExistingClientAsync(
            analysisResult2,
            alphaIdentity,
            clientId);

        // Assert: Vérifier que les contrats sont marqués comme existants et qu'il n'y a pas de doublon
        Assert.Equal(ApplicationOutcome.Success, secondResult.Outcome);
        Assert.Equal(clientId, secondResult.ClientId);
        Assert.False(secondResult.ClientWasCreated);
        Assert.Empty(secondResult.ContractsCreated);
        Assert.Equal(2, secondResult.ContractsAlreadyExisting.Count);
        Assert.Empty(secondResult.ContractsSkipped);

        // Assert: Vérifier dans SQLite qu'il n'y a toujours que 2 contrats (pas de doublons)
        await using var newContext = await _contextFactory.CreateDbContextAsync();
        var contractCount = await newContext.Contracts
            .CountAsync(c => c.ClientId == clientId);

        Assert.Equal(2, contractCount);
    }

    [Fact]
    public async Task GetSelectableClientsAsync_AfterServiceRecreation_ShouldReturnPersistedClientFromSqlite()
    {
        // Arrange: Créer un client via l'application service
        var reader = new BrioCsvFileReader();
        var analyzer = new BrioImportAnalyzer();
        var clientRepository1 = new EfCoreClientRepository(_contextFactory);
        var contractRepository1 = new EfCoreContractRepository(_contextFactory);
        var persistenceService1 = new EfCoreBrioPersistenceService(_contextFactory, Microsoft.Extensions.Logging.Abstractions.NullLogger<EfCoreBrioPersistenceService>.Instance);
        var applicationService1 = new BrioContractApplicationService(clientRepository1, contractRepository1, persistenceService1);

        using var stream = BrioSyntheticFixtureLoader.LoadFixture("BrioSynthetic.ValidCore.csv");
        var readResult = await reader.ReadAsync(stream);
        var analysisResult = await analyzer.AnalyzeAsync(readResult);

        var alphaIdentity = analysisResult.ClientCandidates.Values
            .First(c => c.NormalizedIdentity.StartsWith("INAMI:"))
            .NormalizedIdentity;

        var result = await applicationService1.ApplyWithNewClientAsync(
            analysisResult,
            alphaIdentity,
            Language.French);

        Assert.Equal(ApplicationOutcome.Success, result.Outcome);
        var clientId = result.ClientId!.Value;

        // Act: Recréer ClientSelectionService (simuler une nouvelle session) et récupérer les clients
        var clientRepository2 = new EfCoreClientRepository(_contextFactory);
        var clientSelectionService = new ClientSelectionService(clientRepository2);

        var selectableClients = await clientSelectionService.GetSelectableClientsAsync();

        // Assert: Vérifier que le client persistant est retrouvé depuis SQLite
        Assert.NotEmpty(selectableClients);
        var retrievedClient = selectableClients.FirstOrDefault(c => c.ClientId == clientId);
        Assert.NotNull(retrievedClient);
        Assert.Equal("DR. ALPHA", retrievedClient.FirstName);
        Assert.Equal("SYNTHETIC ALPHA", retrievedClient.LastName);
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
