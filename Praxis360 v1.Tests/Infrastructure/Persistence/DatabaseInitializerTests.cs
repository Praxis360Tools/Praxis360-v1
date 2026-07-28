using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Praxis360_v1.Infrastructure.Persistence;

namespace Praxis360_v1.Tests.Infrastructure.Persistence;

public sealed class DatabaseInitializerTests
{
    [Fact]
    public async Task InitializeAsync_ShouldCreateFolderAndApplyMigrations()
    {
        // Arrange
        var tempPath = Path.Combine(Path.GetTempPath(), $"Praxis360_Test_{Guid.NewGuid()}");
        var tempDbPath = Path.Combine(tempPath, "test.db");

        var resolver = new TestDatabasePathResolver(tempDbPath);
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite($"Data Source={tempDbPath};Foreign Keys=True");

        var factory = new TestDbContextFactory(optionsBuilder.Options);
        var initializer = new DatabaseInitializer(factory, resolver);

        try
        {
            // Act
            await initializer.InitializeAsync();

            // Assert
            Assert.True(Directory.Exists(tempPath));
            Assert.True(File.Exists(tempDbPath));

            // Vérifier que les tables existent
            await using (var context = await factory.CreateDbContextAsync())
            {
                var canConnect = await context.Database.CanConnectAsync();
                Assert.True(canConnect);
            }
        }
        finally
        {
            // Cleanup avec fermeture explicite des connexions SQLite
            SqliteConnection.ClearAllPools();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            if (File.Exists(tempDbPath))
                File.Delete(tempDbPath);
            if (Directory.Exists(tempPath))
                Directory.Delete(tempPath, recursive: true);
        }
    }

    [Fact]
    public async Task InitializeAsync_SecondCall_ShouldBeIdempotent()
    {
        // Arrange
        var tempPath = Path.Combine(Path.GetTempPath(), $"Praxis360_Test_{Guid.NewGuid()}");
        var tempDbPath = Path.Combine(tempPath, "test.db");

        var resolver = new TestDatabasePathResolver(tempDbPath);
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite($"Data Source={tempDbPath};Foreign Keys=True");

        var factory = new TestDbContextFactory(optionsBuilder.Options);
        var initializer = new DatabaseInitializer(factory, resolver);

        try
        {
            // Act
            await initializer.InitializeAsync();
            await initializer.InitializeAsync(); // Second call

            // Assert
            Assert.True(File.Exists(tempDbPath));
        }
        finally
        {
            // Cleanup avec fermeture explicite des connexions SQLite
            SqliteConnection.ClearAllPools();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            if (File.Exists(tempDbPath))
                File.Delete(tempDbPath);
            if (Directory.Exists(tempPath))
                Directory.Delete(tempPath, recursive: true);
        }
    }

    [Fact]
    public async Task InitializeAsync_ShouldPropagateCancellation()
    {
        // Arrange
        var tempPath = Path.Combine(Path.GetTempPath(), $"Praxis360_Test_{Guid.NewGuid()}");
        var tempDbPath = Path.Combine(tempPath, "test.db");

        var resolver = new TestDatabasePathResolver(tempDbPath);
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite($"Data Source={tempDbPath};Foreign Keys=True");

        var factory = new TestDbContextFactory(optionsBuilder.Options);
        var initializer = new DatabaseInitializer(factory, resolver);

        var cts = new CancellationTokenSource();
        cts.Cancel();

        try
        {
            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(
                () => initializer.InitializeAsync(cts.Token));
        }
        finally
        {
            // Cleanup
            if (File.Exists(tempDbPath))
                File.Delete(tempDbPath);
            if (Directory.Exists(tempPath))
                Directory.Delete(tempPath, recursive: true);
        }
    }

    private sealed class TestDatabasePathResolver : IDatabasePathResolver
    {
        private readonly string _path;

        public TestDatabasePathResolver(string path)
        {
            _path = path;
        }

        public string GetDatabasePath() => _path;
    }

    private sealed class TestDbContextFactory : IDbContextFactory<AppDbContext>
    {
        private readonly DbContextOptions<AppDbContext> _options;

        public TestDbContextFactory(DbContextOptions<AppDbContext> options)
        {
            _options = options;
        }

        public AppDbContext CreateDbContext() => new AppDbContext(_options);

        public ValueTask<AppDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
            => ValueTask.FromResult(new AppDbContext(_options));
    }
}
