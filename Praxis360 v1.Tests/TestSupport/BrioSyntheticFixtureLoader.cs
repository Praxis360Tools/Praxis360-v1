namespace Praxis360_v1.Tests.TestSupport;

/// <summary>
/// Loads BRIO synthetic CSV fixtures from the approved fixture directory.
/// Only the 5 approved synthetic fixtures are allowed to be loaded.
/// </summary>
public sealed class BrioSyntheticFixtureLoader
{
    private const string FixtureBasePath = "Fixtures/Brio/Synthetic";

    private static readonly HashSet<string> ApprovedFixtures = new(StringComparer.OrdinalIgnoreCase)
    {
        "BrioSynthetic.ValidCore.csv",
        "BrioSynthetic.WarningMatrix.csv",
        "BrioSynthetic.BlockingMatrix.csv",
        "BrioSynthetic.InvalidColumnCount.csv",
        "BrioSynthetic.Empty.csv"
    };

    public static Stream LoadFixture(string fixtureName)
    {
        if (string.IsNullOrWhiteSpace(fixtureName))
            throw new ArgumentException("Fixture name cannot be null or empty", nameof(fixtureName));

        if (!ApprovedFixtures.Contains(fixtureName))
        {
            throw new InvalidOperationException(
                $"Fixture '{fixtureName}' is not in the approved list. " +
                $"Only the following synthetic fixtures are allowed: {string.Join(", ", ApprovedFixtures)}");
        }

        var fixturePath = Path.Combine(
            AppContext.BaseDirectory,
            FixtureBasePath,
            fixtureName);

        if (!File.Exists(fixturePath))
        {
            throw new FileNotFoundException(
                $"Approved synthetic fixture not found at: {fixturePath}",
                fixturePath);
        }

        return File.OpenRead(fixturePath);
    }

    public static string GetFixturePath(string fixtureName)
    {
        if (!ApprovedFixtures.Contains(fixtureName))
        {
            throw new InvalidOperationException(
                $"Fixture '{fixtureName}' is not in the approved list.");
        }

        return Path.Combine(
            AppContext.BaseDirectory,
            FixtureBasePath,
            fixtureName);
    }
}
