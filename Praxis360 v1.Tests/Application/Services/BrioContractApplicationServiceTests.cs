using Praxis360_v1.Domain.Entities;
using Praxis360.Domain.Types;
using Praxis360_v1.Application.Interfaces;
using Praxis360_v1.Application.Models;
using Praxis360_v1.Application.Services;
using Praxis360_v1.Infrastructure.FileReaders;
using Praxis360_v1.Infrastructure.InMemory;
using Praxis360_v1.Infrastructure.Repositories;
using Praxis360_v1.Tests.TestSupport;

namespace Praxis360_v1.Tests.Application.Services;

public sealed class BrioContractApplicationServiceTests
{
    private readonly IBrioFileReader _reader = new BrioCsvFileReader();
    private readonly IBrioImportAnalyzer _analyzer = new BrioImportAnalyzer();
    private readonly InMemoryPraxis360Store _store = new();
    private readonly IClientRepository _clientRepository;
    private readonly IContractRepository _contractRepository;
    private readonly IBrioPersistenceService _persistenceService;
    private readonly IBrioContractApplicationService _applicationService;

    public BrioContractApplicationServiceTests()
    {
        _clientRepository = new InMemoryClientRepository(_store);
        _contractRepository = new InMemoryContractRepository(_store);
        _persistenceService = new InMemoryBrioPersistenceService(_store);
        _applicationService = new BrioContractApplicationService(_clientRepository, _contractRepository, _persistenceService);
    }

    [Fact]
    public async Task ApplyWithNewClientAsync_ValidCore_SyntheticAlpha_ShouldCreate2Contracts()
    {
        // Arrange
        using var stream = BrioSyntheticFixtureLoader.LoadFixture("BrioSynthetic.ValidCore.csv");
        var readResult = await _reader.ReadAsync(stream);
        var analysisResult = await _analyzer.AnalyzeAsync(readResult);

        var alphaIdentity = analysisResult.ClientCandidates.Values
            .First(c => c.NormalizedIdentity.StartsWith("INAMI:"))
            .NormalizedIdentity;

        // Act
        var result = await _applicationService.ApplyWithNewClientAsync(
            analysisResult,
            alphaIdentity,
            Language.French);

        // Assert
        Assert.Equal(ApplicationOutcome.Success, result.Outcome);
        Assert.NotNull(result.ClientId);
        Assert.True(result.ClientWasCreated);
        Assert.Equal(2, result.ContractsCreated.Count);
        Assert.Empty(result.ContractsAlreadyExisting);
        Assert.Empty(result.ContractsSkipped);
        Assert.Empty(result.ContractsUnresolved);
        Assert.Empty(result.GlobalErrors);
    }

    [Fact]
    public async Task ApplyWithNewClientAsync_ValidCore_Idempotence_ShouldReturn2AlreadyExisting()
    {
        // Arrange
        using var stream = BrioSyntheticFixtureLoader.LoadFixture("BrioSynthetic.ValidCore.csv");
        var readResult = await _reader.ReadAsync(stream);
        var analysisResult = await _analyzer.AnalyzeAsync(readResult);

        var alphaIdentity = analysisResult.ClientCandidates.Values
            .First(c => c.NormalizedIdentity.StartsWith("INAMI:"))
            .NormalizedIdentity;

        // First application
        var firstResult = await _applicationService.ApplyWithNewClientAsync(
            analysisResult,
            alphaIdentity,
            Language.French);

        var clientId = firstResult.ClientId!.Value;

        // Act - Second application to same client
        var secondResult = await _applicationService.ApplyToExistingClientAsync(
            analysisResult,
            alphaIdentity,
            clientId);

        // Assert
        Assert.Equal(ApplicationOutcome.Success, secondResult.Outcome);
        Assert.Equal(clientId, secondResult.ClientId);
        Assert.False(secondResult.ClientWasCreated);
        Assert.Empty(secondResult.ContractsCreated);
        Assert.Equal(2, secondResult.ContractsAlreadyExisting.Count);
        Assert.Empty(secondResult.ContractsUnresolved);
    }

    [Fact]
    public async Task ApplyToExistingClientAsync_ValidCore_SyntheticBeta_ShouldCreate1Contract()
    {
        // Arrange
        var existingClient = new Client(
            Guid.NewGuid(),
            "Marie",
            "SYNTHETIC BETA",
            new DateOnly(1980, 6, 22),
            Language.French);
        await _clientRepository.SaveAsync(existingClient);

        using var stream = BrioSyntheticFixtureLoader.LoadFixture("BrioSynthetic.ValidCore.csv");
        var readResult = await _reader.ReadAsync(stream);
        var analysisResult = await _analyzer.AnalyzeAsync(readResult);

        var betaIdentity = analysisResult.ClientCandidates.Values
            .First(c => c.NormalizedIdentity.Contains("1980-06-22"))
            .NormalizedIdentity;

        // Act
        var result = await _applicationService.ApplyToExistingClientAsync(
            analysisResult,
            betaIdentity,
            existingClient.Id);

        // Assert
        Assert.Equal(ApplicationOutcome.Success, result.Outcome);
        Assert.Equal(existingClient.Id, result.ClientId);
        Assert.False(result.ClientWasCreated);
        Assert.Single(result.ContractsCreated);
        Assert.Empty(result.ContractsAlreadyExisting);
        Assert.Empty(result.ContractsUnresolved);
    }

    [Fact]
    public async Task ApplyWithNewClientAsync_WarningMatrix_SyntheticWarning_ShouldReturnPartialSuccess()
    {
        // Arrange
        using var stream = BrioSyntheticFixtureLoader.LoadFixture("BrioSynthetic.WarningMatrix.csv");
        var readResult = await _reader.ReadAsync(stream);
        var analysisResult = await _analyzer.AnalyzeAsync(readResult);

        var warningIdentity = analysisResult.ClientCandidates.Values
            .First(c => c.LastName.Contains("WARNING"))
            .NormalizedIdentity;

        // Act
        var result = await _applicationService.ApplyWithNewClientAsync(
            analysisResult,
            warningIdentity,
            Language.French);

        // Assert
        Assert.Equal(ApplicationOutcome.PartialSuccess, result.Outcome);
        Assert.NotNull(result.ClientId);
        Assert.True(result.ClientWasCreated);
        Assert.Single(result.ContractsCreated);
        Assert.Empty(result.ContractsAlreadyExisting);
        Assert.Equal(2, result.ContractsUnresolved.Count);
    }

    [Fact]
    public async Task ApplyWithNewClientAsync_WarningMatrix_SyntheticWarning_ShouldContainProductAndStatusUnresolved()
    {
        // Arrange
        using var stream = BrioSyntheticFixtureLoader.LoadFixture("BrioSynthetic.WarningMatrix.csv");
        var readResult = await _reader.ReadAsync(stream);
        var analysisResult = await _analyzer.AnalyzeAsync(readResult);

        var warningIdentity = analysisResult.ClientCandidates.Values
            .First(c => c.LastName.Contains("WARNING"))
            .NormalizedIdentity;

        // Act
        var result = await _applicationService.ApplyWithNewClientAsync(
            analysisResult,
            warningIdentity,
            Language.French);

        // Assert
        var unresolvedCodes = result.ContractsUnresolved
            .SelectMany(u => u.Issues)
            .Select(i => i.Code)
            .ToList();

        Assert.Contains("BRIO_PRODUCT_UNRESOLVED", unresolvedCodes);
    }

    [Fact]
    public async Task ApplyWithNewClientAsync_WarningMatrix_SyntheticUnresolved_ShouldFail()
    {
        // Arrange
        using var stream = BrioSyntheticFixtureLoader.LoadFixture("BrioSynthetic.WarningMatrix.csv");
        var readResult = await _reader.ReadAsync(stream);
        var analysisResult = await _analyzer.AnalyzeAsync(readResult);

        var unresolvedIdentity = analysisResult.ClientCandidates.Values
            .First(c => c.LastName.Contains("UNRESOLVED"))
            .NormalizedIdentity;

        // Act
        var result = await _applicationService.ApplyWithNewClientAsync(
            analysisResult,
            unresolvedIdentity,
            Language.French);

        // Assert
        Assert.Equal(ApplicationOutcome.Failed, result.Outcome);
        Assert.Null(result.ClientId);
        Assert.False(result.ClientWasCreated);
        Assert.Empty(result.ContractsCreated);
        Assert.Empty(result.ContractsAlreadyExisting);
        Assert.Single(result.ContractsUnresolved);
        Assert.Single(result.GlobalErrors);
        Assert.Equal("BRIO_NO_CREATABLE_CONTRACTS", result.GlobalErrors[0].Code);
    }

    [Fact]
    public async Task ApplyWithNewClientAsync_ValidCore_AllContractsShouldHaveCorrectTypes()
    {
        // Arrange
        using var stream = BrioSyntheticFixtureLoader.LoadFixture("BrioSynthetic.ValidCore.csv");
        var readResult = await _reader.ReadAsync(stream);
        var analysisResult = await _analyzer.AnalyzeAsync(readResult);

        var alphaIdentity = analysisResult.ClientCandidates.Values
            .First(c => c.NormalizedIdentity.StartsWith("INAMI:"))
            .NormalizedIdentity;

        // Act
        var result = await _applicationService.ApplyWithNewClientAsync(
            analysisResult,
            alphaIdentity,
            Language.French);

        // Assert
        var clientId = result.ClientId!.Value;
        var contracts = await _contractRepository.GetByClientIdAsync(clientId);

        Assert.Equal(2, contracts.Count);

        var fspsContract = contracts.Single(c =>
            c.ExternalReferences.Any(r =>
                r.SourceSystem == SourceSystem.Brio &&
                r.ReferenceType == ReferenceType.PolicyNumber &&
                r.Value == "SYN-ALPHA-001"));
        Assert.Equal(ContractType.PLCI, fspsContract.Type);

        var eipContract = contracts.Single(c =>
            c.ExternalReferences.Any(r =>
                r.SourceSystem == SourceSystem.Brio &&
                r.ReferenceType == ReferenceType.PolicyNumber &&
                r.Value == "SYN-ALPHA-002"));
        Assert.Equal(ContractType.EIP, eipContract.Type);
    }

    [Fact]
    public async Task ApplyWithNewClientAsync_ValidCore_AllContractsShouldHaveActiveStatus()
    {
        // Arrange
        using var stream = BrioSyntheticFixtureLoader.LoadFixture("BrioSynthetic.ValidCore.csv");
        var readResult = await _reader.ReadAsync(stream);
        var analysisResult = await _analyzer.AnalyzeAsync(readResult);

        var alphaIdentity = analysisResult.ClientCandidates.Values
            .First(c => c.NormalizedIdentity.StartsWith("INAMI:"))
            .NormalizedIdentity;

        // Act
        var result = await _applicationService.ApplyWithNewClientAsync(
            analysisResult,
            alphaIdentity,
            Language.French);

        // Assert
        var clientId = result.ClientId!.Value;
        var contracts = await _contractRepository.GetByClientIdAsync(clientId);
        Assert.All(contracts, c => Assert.Equal(ContractStatus.Active, c.Status));
    }

    [Fact]
    public async Task ApplyWithNewClientAsync_ValidCore_ContractsShouldHaveBrioReferences()
    {
        // Arrange
        using var stream = BrioSyntheticFixtureLoader.LoadFixture("BrioSynthetic.ValidCore.csv");
        var readResult = await _reader.ReadAsync(stream);
        var analysisResult = await _analyzer.AnalyzeAsync(readResult);

        var alphaIdentity = analysisResult.ClientCandidates.Values
            .First(c => c.NormalizedIdentity.StartsWith("INAMI:"))
            .NormalizedIdentity;

        // Act
        var result = await _applicationService.ApplyWithNewClientAsync(
            analysisResult,
            alphaIdentity,
            Language.French);

        var clientId = result.ClientId!.Value;
        var contracts = await _contractRepository.GetByClientIdAsync(clientId);

        // Assert
        Assert.All(contracts, contract =>
        {
            var brioRef = contract.ExternalReferences
                .FirstOrDefault(r => r.SourceSystem == SourceSystem.Brio);
            Assert.NotNull(brioRef);
            Assert.Equal(ReferenceType.PolicyNumber, brioRef.ReferenceType);
            Assert.StartsWith("SYN-", brioRef.Value);
        });
    }
}
