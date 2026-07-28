using Microsoft.EntityFrameworkCore;

namespace Praxis360_v1.Infrastructure.Persistence;

/// <summary>
/// Initialise la base de données SQLite au démarrage de l'application.
/// </summary>
public interface IDatabaseInitializer
{
    /// <summary>
    /// Crée le dossier de la base de données si nécessaire et applique les migrations.
    /// </summary>
    Task InitializeAsync(CancellationToken cancellationToken = default);
}
