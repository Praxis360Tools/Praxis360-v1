using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Praxis360.Domain.Types;
using Praxis360_v1.Domain.Entities;
using Praxis360_v1.Infrastructure.Persistence;
using Praxis360_v1.Infrastructure.Repositories;

namespace Praxis360_v1.Tests.Infrastructure.Repositories;

public sealed class EfCoreClientRepositoryTests : IDisposable
{
    private readonly string _tempDbPath;
    private readonly SqliteConnection _keepAliveConnection;
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public EfCoreClientRepositoryTests()
    {
        _tempDbPath = Path.Combine(Path.GetTempPath(), $"Praxis360_EfCoreClientRepo_{Guid.NewGuid():N}.db");
        var connectionString = $"Data Source={_tempDbPath};Foreign Keys=True";

        _keepAliveConnection = new SqliteConnection(connectionString);
        _keepAliveConnection.Open();

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_keepAliveConnection);

        _contextFactory = new TestDbContextFactory(optionsBuilder.Options);

        using var context = _contextFactory.CreateDbContext();
        context.Database.Migrate();
    }

    public void Dispose()
    {
        _keepAliveConnection?.Dispose();
        SqliteConnection.ClearAllPools();
        GC.Collect();
        GC.WaitForPendingFinalizers();

        if (File.Exists(_tempDbPath))
        {
            try { File.Delete(_tempDbPath); } catch { }
        }
    }

    [Fact]
    public async Task SaveAsync_ThenGetByIdAsync_ShouldReturnClient()
    {
        var repository = new EfCoreClientRepository(_contextFactory);
        var client = new Client(
            id: Guid.NewGuid(),
            firstName: "Jean",
            lastName: "Dupont",
            dateOfBirth: new DateOnly(1980, 5, 15),
            preferredLanguage: Language.French,
            address: null
        );

        await repository.SaveAsync(client);

        var retrieved = await repository.GetByIdAsync(client.Id);

        Assert.NotNull(retrieved);
        Assert.Equal(client.Id, retrieved.Id);
        Assert.Equal("Jean", retrieved.FirstName);
        Assert.Equal("Dupont", retrieved.LastName);
        Assert.Equal(new DateOnly(1980, 5, 15), retrieved.DateOfBirth);
    }

    [Fact]
    public async Task GetByIdAsync_NotFound_ShouldReturnNull()
    {
        var repository = new EfCoreClientRepository(_contextFactory);
        var retrieved = await repository.GetByIdAsync(Guid.NewGuid());
        Assert.Null(retrieved);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllClients()
    {
        var repository = new EfCoreClientRepository(_contextFactory);
        var client1 = new Client(Guid.NewGuid(), "Alice", "Martin", new DateOnly(1985, 3, 10), Language.French, null);
        var client2 = new Client(Guid.NewGuid(), "Bob", "Durand", new DateOnly(1990, 7, 20), Language.Dutch, null);

        await repository.SaveAsync(client1);
        await repository.SaveAsync(client2);

        var all = await repository.GetAllAsync();

        Assert.Equal(2, all.Count);
        Assert.Contains(all, c => c.Id == client1.Id);
        Assert.Contains(all, c => c.Id == client2.Id);
    }

    [Fact]
    public async Task SearchByIdentityAsync_ByFirstNameAndLastName_ShouldReturnMatch()
    {
        var repository = new EfCoreClientRepository(_contextFactory);
        var client = new Client(Guid.NewGuid(), "Marie", "Lecomte", new DateOnly(1975, 11, 5), Language.French, null);
        await repository.SaveAsync(client);

        var results = await repository.SearchByIdentityAsync("Marie", "Lecomte", null);

        Assert.Single(results);
        Assert.Equal(client.Id, results.First().Id);
    }

    [Fact]
    public async Task SearchByIdentityAsync_ByDateOfBirth_ShouldReturnMatch()
    {
        var repository = new EfCoreClientRepository(_contextFactory);
        var dob = new DateOnly(1982, 4, 12);
        var client = new Client(Guid.NewGuid(), "Pierre", "Lemoine", dob, Language.French, null);
        await repository.SaveAsync(client);

        var results = await repository.SearchByIdentityAsync("", "", dob);

        Assert.Single(results);
        Assert.Equal(client.Id, results.First().Id);
    }

    [Fact]
    public async Task UpdateAsync_ShouldPersistChanges()
    {
        var repository = new EfCoreClientRepository(_contextFactory);
        var client = new Client(Guid.NewGuid(), "Luc", "Bernard", new DateOnly(1970, 8, 25), Language.French, null);
        await repository.SaveAsync(client);

        var updated = new Client(client.Id, "Luc", "Bernard", new DateOnly(1970, 8, 25), Language.Dutch, null);
        await repository.UpdateAsync(updated);

        var retrieved = await repository.GetByIdAsync(client.Id);
        Assert.NotNull(retrieved);
        Assert.Equal(Language.Dutch, retrieved.PreferredLanguage);
    }

    [Fact]
    public async Task SaveAsync_VisibleInNewContext()
    {
        var repository = new EfCoreClientRepository(_contextFactory);
        var client = new Client(Guid.NewGuid(), "Sophie", "Dubois", new DateOnly(1988, 2, 14), Language.French, null);
        await repository.SaveAsync(client);

        var newRepository = new EfCoreClientRepository(_contextFactory);
        var retrieved = await newRepository.GetByIdAsync(client.Id);

        Assert.NotNull(retrieved);
        Assert.Equal("Sophie", retrieved.FirstName);
    }

    [Fact]
    public async Task SearchByIdentityAsync_CaseInsensitiveFirstName_ShouldReturnMatch()
    {
        var repository = new EfCoreClientRepository(_contextFactory);
        var client = new Client(Guid.NewGuid(), "Marie", "Lecomte", new DateOnly(1975, 11, 5), Language.French, null);
        await repository.SaveAsync(client);

        var results = await repository.SearchByIdentityAsync("marie", "Lecomte", null);

        Assert.Single(results);
        Assert.Equal(client.Id, results.First().Id);
    }

    [Fact]
    public async Task SearchByIdentityAsync_CaseInsensitiveLastName_ShouldReturnMatch()
    {
        var repository = new EfCoreClientRepository(_contextFactory);
        var client = new Client(Guid.NewGuid(), "Marie", "Lecomte", new DateOnly(1975, 11, 5), Language.French, null);
        await repository.SaveAsync(client);

        var results = await repository.SearchByIdentityAsync("Marie", "LECOMTE", null);

        Assert.Single(results);
        Assert.Equal(client.Id, results.First().Id);
    }

    [Fact]
    public async Task SearchByIdentityAsync_CaseInsensitiveBoth_ShouldReturnMatch()
    {
        var repository = new EfCoreClientRepository(_contextFactory);
        var client = new Client(Guid.NewGuid(), "Marie", "Lecomte", new DateOnly(1975, 11, 5), Language.French, null);
        await repository.SaveAsync(client);

        var results = await repository.SearchByIdentityAsync("MARIE", "lecomte", null);

        Assert.Single(results);
        Assert.Equal(client.Id, results.First().Id);
    }

    [Fact]
    public async Task SearchByIdentityAsync_WithTrim_ShouldReturnMatch()
    {
        var repository = new EfCoreClientRepository(_contextFactory);
        var client = new Client(Guid.NewGuid(), "Marie", "Lecomte", new DateOnly(1975, 11, 5), Language.French, null);
        await repository.SaveAsync(client);

        var results = await repository.SearchByIdentityAsync("  Marie  ", "  Lecomte  ", null);

        Assert.Single(results);
        Assert.Equal(client.Id, results.First().Id);
    }

    private sealed class TestDbContextFactory : IDbContextFactory<AppDbContext>
    {
        private readonly DbContextOptions<AppDbContext> _options;

        public TestDbContextFactory(DbContextOptions<AppDbContext> options)
        {
            _options = options;
        }

        public AppDbContext CreateDbContext() => new(_options);
    }
}
