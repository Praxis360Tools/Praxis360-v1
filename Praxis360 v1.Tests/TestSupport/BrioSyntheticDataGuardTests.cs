namespace Praxis360_v1.Tests.TestSupport;

/// <summary>
/// Tests to validate that all approved synthetic fixtures contain only fictional data
/// and no real customer information.
/// </summary>
public sealed class BrioSyntheticDataGuardTests
{
    [Theory]
    [InlineData("BrioSynthetic.ValidCore.csv")]
    [InlineData("BrioSynthetic.WarningMatrix.csv")]
    [InlineData("BrioSynthetic.BlockingMatrix.csv")]
    [InlineData("BrioSynthetic.InvalidColumnCount.csv")]
    [InlineData("BrioSynthetic.Empty.csv")]
    public void AllApprovedFixtures_ShouldPassDataGuardValidation(string fixtureName)
    {
        // Arrange
        var fixturePath = BrioSyntheticFixtureLoader.GetFixturePath(fixtureName);

        // Act
        var result = BrioSyntheticDataGuard.ValidateFixture(fixturePath, fixtureName);

        // Assert
        Assert.True(result.IsValid,
            $"Fixture {fixtureName} failed validation:\n{result}");
    }

    [Fact]
    public void DataGuard_ShouldRejectNonApprovedFixtureName()
    {
        // Arrange
        var invalidName = "RealBrioExport.csv";

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() =>
            BrioSyntheticFixtureLoader.LoadFixture(invalidName));

        Assert.Contains("not in the approved list", ex.Message);
    }

    [Fact]
    public void DataGuard_ShouldDetectMissingFictionalKeywords()
    {
        // Arrange
        var builder = new BrioSyntheticRowBuilder();
        builder.WithInsuredLastName("Dupont");
        builder.WithInsuredFirstName("Jean");
        builder.WithInsuredEmail("jean@real-company.be");
        builder.WithPolicyNumberPrimary("SYN-12345");
        var testLine = builder.BuildCsvLine();
        var tempPath = Path.GetTempFileName();

        try
        {
            var header = BrioSyntheticRowBuilder.BuildHeader();
            File.WriteAllText(tempPath, header + "\n" + testLine);

            // Act
            var result = BrioSyntheticDataGuard.ValidateFixture(tempPath, "test.csv");

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Contains("does not contain an approved fictional keyword"));
        }
        finally
        {
            File.Delete(tempPath);
        }
    }

    [Fact]
    public void DataGuard_ShouldDetectNonApprovedEmailDomain()
    {
        // Arrange
        var builder = new BrioSyntheticRowBuilder();
        builder.WithInsuredLastName("SYNTHETIC");
        builder.WithInsuredFirstName("TEST");
        builder.WithInsuredEmail("real.email@company.com");
        builder.WithPolicyNumberPrimary("SYN-12345");
        var testLine = builder.BuildCsvLine();
        var tempPath = Path.GetTempFileName();

        try
        {
            var header = BrioSyntheticRowBuilder.BuildHeader();
            File.WriteAllText(tempPath, header + "\n" + testLine);

            // Act
            var result = BrioSyntheticDataGuard.ValidateFixture(tempPath, "test.csv");

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Contains("Email domain") && e.Contains("not approved"));
        }
        finally
        {
            File.Delete(tempPath);
        }
    }

    [Fact]
    public void DataGuard_ShouldDetectMissingSynPrefixInPolicyNumber()
    {
        // Arrange
        var builder = new BrioSyntheticRowBuilder();
        builder.WithInsuredLastName("SYNTHETIC");
        builder.WithInsuredFirstName("TEST");
        builder.WithInsuredEmail("test@example.test");
        builder.WithPolicyNumberPrimary("12345678");
        builder.WithPolicyNumberRepeated("12345678");
        var testLine = builder.BuildCsvLine();
        var tempPath = Path.GetTempFileName();

        try
        {
            var header = BrioSyntheticRowBuilder.BuildHeader();
            File.WriteAllText(tempPath, header + "\n" + testLine);

            // Act
            var result = BrioSyntheticDataGuard.ValidateFixture(tempPath, "test.csv");

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Contains("must start with 'SYN-' prefix"));
        }
        finally
        {
            File.Delete(tempPath);
        }
    }

    [Fact]
    public void RowBuilder_ShouldHandleNullCellWithoutException()
    {
        // Arrange
        var builder = new BrioSyntheticRowBuilder();
        builder.WithInsuredLastName("SYNTHETIC");
        builder.WithCell(5, null!);

        // Act
        var csvLine = builder.BuildCsvLine();

        // Assert
        Assert.NotNull(csvLine);
        var cells = csvLine.Split(';');
        Assert.Equal(62, cells.Length);
        Assert.Equal(string.Empty, cells[5]);
    }

    [Fact]
    public void DataGuard_ShouldDetectInvalidColumnCount_CaseInsensitive()
    {
        // Arrange
        var tempPath = Path.GetTempFileName();

        try
        {
            // Create a CSV with wrong column count but with "invalidcolumncount" in lowercase
            File.WriteAllText(tempPath, "Col1;Col2;Col3\nVal1;Val2;Val3");

            // Act
            var result = BrioSyntheticDataGuard.ValidateFixture(tempPath, "BrioSynthetic.invalidcolumncount.csv");

            // Assert - Should NOT produce error about column count
            Assert.DoesNotContain(result.Errors, e => e.Contains("Header must contain exactly 62 columns"));
        }
        finally
        {
            File.Delete(tempPath);
        }
    }

    [Fact]
    public void DataGuard_ShouldRejectOversizedFile()
    {
        // Arrange
        var tempPath = Path.GetTempFileName();

        try
        {
            // Create a file larger than 50,000 bytes
            var largeContent = new string('X', 51000);
            File.WriteAllText(tempPath, largeContent);

            // Act
            var result = BrioSyntheticDataGuard.ValidateFixture(tempPath, "oversized.csv");

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Contains("too large") && e.Contains("50000"));
        }
        finally
        {
            File.Delete(tempPath);
        }
    }

    [Fact]
    public void FixtureLoader_GetFixturePath_ShouldRejectNullName()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            BrioSyntheticFixtureLoader.GetFixturePath(null!));

        Assert.Contains("Fixture name cannot be null, empty, or whitespace.", ex.Message);
        Assert.Equal("fixtureName", ex.ParamName);
    }

    [Fact]
    public void FixtureLoader_GetFixturePath_ShouldRejectEmptyName()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            BrioSyntheticFixtureLoader.GetFixturePath(""));

        Assert.Contains("Fixture name cannot be null, empty, or whitespace.", ex.Message);
        Assert.Equal("fixtureName", ex.ParamName);
    }

    [Fact]
    public void FixtureLoader_GetFixturePath_ShouldRejectWhitespaceName()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            BrioSyntheticFixtureLoader.GetFixturePath("   "));

        Assert.Contains("Fixture name cannot be null, empty, or whitespace.", ex.Message);
        Assert.Equal("fixtureName", ex.ParamName);
    }
}
