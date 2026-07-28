namespace Praxis360_v1.Infrastructure.Persistence;

/// <summary>
/// Résout le chemin de la base de données SQLite runtime dans LocalApplicationData.
/// </summary>
public sealed class LocalAppDataDatabasePathResolver : IDatabasePathResolver
{
    private const string AppFolderName = "Praxis360";
    private const string DatabaseFileName = "praxis360.db";

    public string GetDatabasePath()
    {
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appFolder = Path.Combine(localAppData, AppFolderName);
        var databasePath = Path.Combine(appFolder, DatabaseFileName);
        return databasePath;
    }
}
