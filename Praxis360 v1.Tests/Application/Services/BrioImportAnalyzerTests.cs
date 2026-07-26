using Praxis360_v1.Application.Interfaces;
using Praxis360_v1.Application.Models;
using Praxis360_v1.Application.Services;
using Praxis360_v1.Infrastructure.FileReaders;
using Praxis360_v1.Tests.TestSupport;

namespace Praxis360_v1.Tests.Application.Services;

public sealed class BrioImportAnalyzerTests
{
    private readonly IBrioFileReader _reader = new BrioCsvFileReader();
    private readonly IBrioImportAnalyzer _analyzer = new BrioImportAnalyzer();

    [Fact]
    public async Task AnalyzeAsync_ValidCore_ShouldReturn3ClientsAnd4Contracts()
    {
        // Arrange
        using var stream = BrioSyntheticFixtureLoader.LoadFixture("BrioSynthetic.ValidCore.csv");
        var readResult = await _reader.ReadAsync(stream);

        // Act
        var result = await _analyzer.AnalyzeAsync(readResult);

        // Assert
        Assert.Equal(4, result.AnalyzedLines.Count);
        Assert.Equal(3, result.ClientCandidates.Count);
        Assert.Equal(4, result.ContractCandidates.Count);
        Assert.Equal(0, result.WarningCount);
        Assert.Equal(0, result.BlockingErrorCount);
        Assert.False(result.HasBlockingErrors);
        Assert.True(result.CanProceed);
    }

    [Fact]
    public async Task AnalyzeAsync_ValidCore_ShouldIdentifyClientsCorrectly()
    {
        // Arrange
        using var stream = BrioSyntheticFixtureLoader.LoadFixture("BrioSynthetic.ValidCore.csv");
        var readResult = await _reader.ReadAsync(stream);

        // Act
        var result = await _analyzer.AnalyzeAsync(readResult);

        // Assert
        var clientIdentities = result.ClientCandidates.Values.Select(c => c.NormalizedIdentity).ToList();
        Assert.Contains(clientIdentities, id => id.StartsWith("INAMI:"));
        Assert.Contains(clientIdentities, id => id.Contains("|1980-06-22"));
        Assert.Contains(clientIdentities, id => id.Contains("pierre.gamma@example.test"));
    }

    [Fact]
    public async Task AnalyzeAsync_WarningMatrix_ShouldReturn2DuplicateWarnings()
    {
        // Arrange
        using var stream = BrioSyntheticFixtureLoader.LoadFixture("BrioSynthetic.WarningMatrix.csv");
        var readResult = await _reader.ReadAsync(stream);

        // Act
        var result = await _analyzer.AnalyzeAsync(readResult);

        // Assert
        var duplicateWarnings = result.AllIssues
            .Where(i => i.Code == "BRIO_EXACT_DUPLICATE" && i.Severity == ImportIssueSeverity.Warning)
            .ToList();

        Assert.Equal(2, duplicateWarnings.Count);
    }

    [Fact]
    public async Task AnalyzeAsync_WarningMatrix_ShouldReturn2ProductUnknownWarnings()
    {
        // Arrange
        using var stream = BrioSyntheticFixtureLoader.LoadFixture("BrioSynthetic.WarningMatrix.csv");
        var readResult = await _reader.ReadAsync(stream);

        // Act
        var result = await _analyzer.AnalyzeAsync(readResult);

        // Assert
        var productWarnings = result.AllIssues
            .Where(i => i.Code == "BRIO_PRODUCT_UNKNOWN" && i.Severity == ImportIssueSeverity.Warning)
            .ToList();

        Assert.Equal(2, productWarnings.Count);
    }

    [Fact]
    public async Task AnalyzeAsync_WarningMatrix_ShouldHaveNoBlockingErrors()
    {
        // Arrange
        using var stream = BrioSyntheticFixtureLoader.LoadFixture("BrioSynthetic.WarningMatrix.csv");
        var readResult = await _reader.ReadAsync(stream);

        // Act
        var result = await _analyzer.AnalyzeAsync(readResult);

        // Assert
        Assert.False(result.HasBlockingErrors);
        Assert.True(result.CanProceed);
        Assert.Equal(6, result.AnalyzedLines.Count);
        Assert.Equal(2, result.ClientCandidates.Count);
        Assert.Equal(4, result.ContractCandidates.Count);
    }

    [Fact]
    public async Task AnalyzeAsync_BlockingMatrix_ShouldHave3BlockingErrors()
    {
        // Arrange
        using var stream = BrioSyntheticFixtureLoader.LoadFixture("BrioSynthetic.BlockingMatrix.csv");
        var readResult = await _reader.ReadAsync(stream);

        // Act
        var result = await _analyzer.AnalyzeAsync(readResult);

        // Assert
        var blockingErrors = result.AllIssues
            .Where(i => i.Severity == ImportIssueSeverity.BlockingError)
            .ToList();

        Assert.Equal(3, blockingErrors.Count);
        Assert.True(result.HasBlockingErrors);
        Assert.False(result.CanProceed);
    }

    [Fact]
    public async Task AnalyzeAsync_BlockingMatrix_ShouldDetectClientNotIdentifiable()
    {
        // Arrange
        using var stream = BrioSyntheticFixtureLoader.LoadFixture("BrioSynthetic.BlockingMatrix.csv");
        var readResult = await _reader.ReadAsync(stream);

        // Act
        var result = await _analyzer.AnalyzeAsync(readResult);

        // Assert
        Assert.Contains(result.AllIssues, i => i.Code == "BRIO_CLIENT_NOT_IDENTIFIABLE");
    }

    [Fact]
    public async Task AnalyzeAsync_BlockingMatrix_ShouldDetectBlockingErrors()
    {
        // Arrange
        using var stream = BrioSyntheticFixtureLoader.LoadFixture("BrioSynthetic.BlockingMatrix.csv");
        var readResult = await _reader.ReadAsync(stream);

        // Act
        var result = await _analyzer.AnalyzeAsync(readResult);

        // Assert
        Assert.True(result.HasBlockingErrors);
        Assert.Contains(result.AllIssues, i => i.Code == "BRIO_CLIENT_NOT_IDENTIFIABLE");
    }

    [Fact]
    public async Task AnalyzeAsync_BlockingMatrix_ShouldCreateNoContractCandidates()
    {
        // Arrange
        using var stream = BrioSyntheticFixtureLoader.LoadFixture("BrioSynthetic.BlockingMatrix.csv");
        var readResult = await _reader.ReadAsync(stream);

        // Act
        var result = await _analyzer.AnalyzeAsync(readResult);

        // Assert
        Assert.Empty(result.ContractCandidates);
        Assert.Empty(result.ClientCandidates);
    }

    [Fact]
    public async Task AnalyzeAsync_InvalidColumnCount_ShouldConvertStructuralErrorsToBlockingErrors()
    {
        // Arrange
        using var stream = BrioSyntheticFixtureLoader.LoadFixture("BrioSynthetic.InvalidColumnCount.csv");
        var readResult = await _reader.ReadAsync(stream);

        // Act
        var result = await _analyzer.AnalyzeAsync(readResult);

        // Assert
        Assert.True(result.HasBlockingErrors);
        Assert.False(result.CanProceed);
        var blockingErrors = result.AllIssues.Where(i => i.Severity == ImportIssueSeverity.BlockingError).ToList();
        Assert.Single(blockingErrors);
        Assert.Equal("BRIO_STRUCTURAL_ERROR", blockingErrors[0].Code);
    }

    [Fact]
    public async Task AnalyzeAsync_EmptyFile_ShouldReturnStructuralError()
    {
        // Arrange
        using var stream = BrioSyntheticFixtureLoader.LoadFixture("BrioSynthetic.Empty.csv");
        var readResult = await _reader.ReadAsync(stream);

        // Act
        var result = await _analyzer.AnalyzeAsync(readResult);

        // Assert
        Assert.True(result.HasBlockingErrors);
        Assert.False(result.CanProceed);
        var blockingErrors = result.AllIssues.Where(i => i.Severity == ImportIssueSeverity.BlockingError).ToList();
        Assert.Single(blockingErrors);
        Assert.Equal("BRIO_STRUCTURAL_ERROR", blockingErrors[0].Code);
    }

    [Fact]
    public async Task AnalyzeAsync_WarningMatrix_DuplicateLinesShouldGroupIntoSingleContract()
    {
        // Arrange
        using var stream = BrioSyntheticFixtureLoader.LoadFixture("BrioSynthetic.WarningMatrix.csv");
        var readResult = await _reader.ReadAsync(stream);

        // Act
        var result = await _analyzer.AnalyzeAsync(readResult);

        // Assert
        var warningClient = result.ContractCandidates.FirstOrDefault(c => c.NormalizedPolicyNumber == "SYN-WARNING-001");
        Assert.NotNull(warningClient);
        Assert.Equal(3, warningClient.SourceLineNumbers.Count);
    }
}
