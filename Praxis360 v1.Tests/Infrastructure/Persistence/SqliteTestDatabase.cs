using System;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Praxis360_v1.Infrastructure.Persistence;

namespace Praxis360_v1.Tests.Infrastructure.Persistence;

public sealed class SqliteTestDatabase : IAsyncDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<AppDbContext> _options;

    public SqliteTestDatabase()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        // Enable foreign keys
        using var enableFkCommand = _connection.CreateCommand();
        enableFkCommand.CommandText = "PRAGMA foreign_keys = ON;";
        enableFkCommand.ExecuteNonQuery();

        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .EnableSensitiveDataLogging()
            .Options;
    }

    public AppDbContext CreateContext()
    {
        return new AppDbContext(_options);
    }

    public async ValueTask DisposeAsync()
    {
        await _connection.DisposeAsync();
    }
}
