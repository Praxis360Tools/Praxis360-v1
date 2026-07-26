using Praxis360_v1.Application.Interfaces;
using Praxis360_v1.Infrastructure.FileReaders;
using Praxis360_v1.Tests.TestSupport;

namespace Praxis360_v1.Tests.Infrastructure.FileReaders;

public sealed class BrioCsvFileReaderTests
{
    private readonly IBrioFileReader _reader = new BrioCsvFileReader();

    [Fact]
    public async Task ReadAsync_ValidCore_ShouldRead4Lines()
    {
        // Arrange
        using var stream = BrioSyntheticFixtureLoader.LoadFixture("BrioSynthetic.ValidCore.csv");

        // Act
        var result = await _reader.ReadAsync(stream);

        // Assert
        Assert.Equal(4, result.Lines.Count);
        Assert.Empty(result.StructuralErrors);
        Assert.False(result.HasStructuralErrors);
        Assert.Equal(62, result.ExpectedColumnCount);
    }

    [Fact]
    public async Task ReadAsync_EmptyFile_ShouldReturnEmptyError()
    {
        // Arrange
        using var stream = BrioSyntheticFixtureLoader.LoadFixture("BrioSynthetic.Empty.csv");

        // Act
        var result = await _reader.ReadAsync(stream);

        // Assert
        Assert.Empty(result.Lines);
        Assert.Single(result.StructuralErrors);
        Assert.Contains("File is empty", result.StructuralErrors[0]);
        Assert.True(result.HasStructuralErrors);
    }

    [Fact]
    public async Task ReadAsync_InvalidColumnCount_ShouldRejectLine()
    {
        // Arrange
        using var stream = BrioSyntheticFixtureLoader.LoadFixture("BrioSynthetic.InvalidColumnCount.csv");

        // Act
        var result = await _reader.ReadAsync(stream);

        // Assert
        Assert.Empty(result.Lines);
        Assert.Single(result.StructuralErrors);
        Assert.Contains("expected 62 columns but found", result.StructuralErrors[0]);
        Assert.True(result.HasStructuralErrors);
    }

    [Fact]
    public async Task ReadAsync_WarningMatrix_ShouldRead6Lines()
    {
        // Arrange
        using var stream = BrioSyntheticFixtureLoader.LoadFixture("BrioSynthetic.WarningMatrix.csv");

        // Act
        var result = await _reader.ReadAsync(stream);

        // Assert
        Assert.Equal(6, result.Lines.Count);
        Assert.Empty(result.StructuralErrors);
        Assert.False(result.HasStructuralErrors);
    }

    [Fact]
    public async Task ReadAsync_BlockingMatrix_ShouldRead3Lines()
    {
        // Arrange
        using var stream = BrioSyntheticFixtureLoader.LoadFixture("BrioSynthetic.BlockingMatrix.csv");

        // Act
        var result = await _reader.ReadAsync(stream);

        // Assert
        Assert.Equal(3, result.Lines.Count);
        Assert.Empty(result.StructuralErrors);
        Assert.False(result.HasStructuralErrors);
    }

    [Fact]
    public async Task ReadAsync_NullStream_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _reader.ReadAsync(null!));
    }

    [Fact]
    public async Task ReadAsync_ValidCore_AllLinesShouldHave62Columns()
    {
        // Arrange
        using var stream = BrioSyntheticFixtureLoader.LoadFixture("BrioSynthetic.ValidCore.csv");

        // Act
        var result = await _reader.ReadAsync(stream);

        // Assert
        foreach (var line in result.Lines)
        {
            Assert.Equal(62, line.Cells.Count);
        }
    }
}
