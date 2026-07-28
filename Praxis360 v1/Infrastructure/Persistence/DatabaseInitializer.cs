using Microsoft.EntityFrameworkCore;

namespace Praxis360_v1.Infrastructure.Persistence;

/// <summary>
/// Initialise la base de données SQLite au démarrage de l'application.
/// </summary>
public sealed class DatabaseInitializer : IDatabaseInitializer
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;
    private readonly IDatabasePathResolver _pathResolver;

    public DatabaseInitializer(
        IDbContextFactory<AppDbContext> contextFactory,
        IDatabasePathResolver pathResolver)
    {
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        _pathResolver = pathResolver ?? throw new ArgumentNullException(nameof(pathResolver));
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        // Créer le dossier parent si nécessaire
        var databasePath = _pathResolver.GetDatabasePath();
        var directory = Path.GetDirectoryName(databasePath);

        if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Appliquer les migrations
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        await context.Database.MigrateAsync(cancellationToken);
    }
}
