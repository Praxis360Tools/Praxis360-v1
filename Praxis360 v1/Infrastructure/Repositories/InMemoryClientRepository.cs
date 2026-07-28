using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Praxis360_v1.Application.Interfaces;
using Praxis360_v1.Domain.Entities;
using Praxis360_v1.Infrastructure.InMemory;

namespace Praxis360_v1.Infrastructure.Repositories;

public sealed class InMemoryClientRepository : IClientRepository
{
    private readonly InMemoryPraxis360Store _store;

    public InMemoryClientRepository(InMemoryPraxis360Store store)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
    }

    // Constructeur sans paramètre pour compatibilité avec les tests existants
    public InMemoryClientRepository()
        : this(new InMemoryPraxis360Store())
    {
    }

    public Task<Client?> GetByIdAsync(Guid id)
    {
        var client = _store.GetClient(id);
        return Task.FromResult(client);
    }

    public Task<IReadOnlyCollection<Client>> GetAllAsync()
    {
        var clients = _store.GetAllClients();
        return Task.FromResult<IReadOnlyCollection<Client>>(clients);
    }

    public Task<IReadOnlyCollection<Client>> SearchByIdentityAsync(string firstName, string lastName, DateOnly? dateOfBirth)
    {
        var query = _store.GetAllClients().AsEnumerable();

        if (!string.IsNullOrWhiteSpace(firstName))
        {
            var searchFirstName = firstName.Trim();
            query = query.Where(c => c.FirstName.Equals(searchFirstName, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(lastName))
        {
            var searchLastName = lastName.Trim();
            query = query.Where(c => c.LastName.Equals(searchLastName, StringComparison.OrdinalIgnoreCase));
        }

        if (dateOfBirth.HasValue)
        {
            query = query.Where(c => c.DateOfBirth == dateOfBirth.Value);
        }

        var results = query.ToList();
        return Task.FromResult<IReadOnlyCollection<Client>>(results);
    }

    public Task SaveAsync(Client client)
    {
        if (client is null)
            throw new ArgumentNullException(nameof(client));

        _store.AddClient(client);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Client client)
    {
        if (client is null)
            throw new ArgumentNullException(nameof(client));

        _store.UpdateClient(client);
        return Task.CompletedTask;
    }
}
