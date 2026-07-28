namespace Praxis360_v1.Infrastructure.Persistence;

/// <summary>
/// Résout le chemin complet de la base de données SQLite runtime.
/// </summary>
public interface IDatabasePathResolver
{
    /// <summary>
    /// Retourne le chemin complet du fichier de base de données SQLite.
    /// </summary>
    string GetDatabasePath();
}
