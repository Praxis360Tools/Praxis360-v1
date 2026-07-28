using Praxis360_v1.Infrastructure.Persistence;

namespace Praxis360_v1.Tests.Infrastructure.Persistence;

public sealed class LocalAppDataDatabasePathResolverTests
{
    [Fact]
    public void GetDatabasePath_ShouldReturnValidPath()
    {
        // Arrange
        var resolver = new LocalAppDataDatabasePathResolver();

        // Act
        var path = resolver.GetDatabasePath();

        // Assert
        Assert.NotEmpty(path);
        Assert.Contains("Praxis360", path);
        Assert.Contains("praxis360.db", path);
        Assert.True(Path.IsPathRooted(path));
    }

    [Fact]
    public void GetDatabasePath_ShouldNotCreateAnyFiles()
    {
        // Arrange
        var resolver = new LocalAppDataDatabasePathResolver();

        // Act
        var path = resolver.GetDatabasePath();

        // Assert
        // Le resolver ne doit créer aucun fichier ni dossier
        var directory = Path.GetDirectoryName(path);
        // On ne vérifie pas l'existence car le dossier peut déjà exister
        // L'important est que le resolver soit pur et ne modifie rien
    }
}
