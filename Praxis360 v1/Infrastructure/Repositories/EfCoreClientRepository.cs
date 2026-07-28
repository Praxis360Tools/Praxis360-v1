using Microsoft.EntityFrameworkCore;
using Praxis360_v1.Application.Interfaces;
using Praxis360_v1.Domain.Entities;
using Praxis360_v1.Infrastructure.Persistence;
using Praxis360_v1.Infrastructure.Persistence.Mappers;

namespace Praxis360_v1.Infrastructure.Repositories;

/// <summary>
/// Repository EF Core pour les clients.
/// </summary>
public sealed class EfCoreClientRepository : IClientRepository
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public EfCoreClientRepository(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    public async Task<Client?> GetByIdAsync(Guid id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var entity = await context.Clients
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        return entity is null ? null : ClientPersistenceMapper.ToDomain(entity);
    }

    public async Task<IReadOnlyCollection<Client>> GetAllAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var entities = await context.Clients
            .AsNoTracking()
            .ToListAsync();

        return entities.Select(ClientPersistenceMapper.ToDomain).ToList();
    }

    public async Task<IReadOnlyCollection<Client>> SearchByIdentityAsync(string firstName, string lastName, DateOnly? dateOfBirth)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var query = context.Clients.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(firstName))
        {
            var searchFirstName = firstName.Trim();
            query = query.Where(c => c.FirstName == searchFirstName);
        }

        if (!string.IsNullOrWhiteSpace(lastName))
        {
            var searchLastName = lastName.Trim();
            query = query.Where(c => c.LastName == searchLastName);
        }

        if (dateOfBirth.HasValue)
        {
            query = query.Where(c => c.DateOfBirth == dateOfBirth.Value);
        }

        var entities = await query.ToListAsync();
        return entities.Select(ClientPersistenceMapper.ToDomain).ToList();
    }

    public async Task SaveAsync(Client client)
    {
        if (client is null)
            throw new ArgumentNullException(nameof(client));

        await using var context = await _contextFactory.CreateDbContextAsync();
        var entity = ClientPersistenceMapper.ToEntity(client);
        context.Clients.Add(entity);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Client client)
    {
        if (client is null)
            throw new ArgumentNullException(nameof(client));

        await using var context = await _contextFactory.CreateDbContextAsync();
        var entity = ClientPersistenceMapper.ToEntity(client);
        context.Clients.Update(entity);
        await context.SaveChangesAsync();
    }
}
