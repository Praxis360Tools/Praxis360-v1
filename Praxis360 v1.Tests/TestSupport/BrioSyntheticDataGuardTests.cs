namespace Praxis360_v1.Tests.TestSupport;

/// <summary>
/// Tests to validate that all approved synthetic fixtures contain only fictional data
/// and no real customer information.
/// </summary>
public sealed class BrioSyntheticDataGuardTests
{
    private static readonly string[] ApprovedFixtures = new[]
    {
        "BrioSynthetic.ValidCore.csv",
        "BrioSynthetic.WarningMatrix.csv",
        "BrioSynthetic.BlockingMatrix.csv",
        "BrioSynthetic.InvalidColumnCount.csv",
        "BrioSynthetic.Empty.csv"
    };

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
}
