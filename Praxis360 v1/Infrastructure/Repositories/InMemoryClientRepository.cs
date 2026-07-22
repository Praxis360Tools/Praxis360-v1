using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Praxis360_v1.Application.Interfaces;
using Praxis360_v1.Domain.Entities;

namespace Praxis360_v1.Infrastructure.Repositories;

public sealed class InMemoryClientRepository : IClientRepository
{
    private readonly Dictionary<Guid, Client> _clients = new();
    private readonly object _lock = new();

    public Task<Client?> GetByIdAsync(Guid id)
    {
        lock (_lock)
        {
            _clients.TryGetValue(id, out var client);
            return Task.FromResult(client);
        }
    }

    public Task<IReadOnlyCollection<Client>> GetAllAsync()
    {
        lock (_lock)
        {
            var clients = _clients.Values.ToList();
            return Task.FromResult<IReadOnlyCollection<Client>>(clients);
        }
    }

    public Task<IReadOnlyCollection<Client>> SearchByIdentityAsync(string firstName, string lastName, DateOnly? dateOfBirth)
    {
        lock (_lock)
        {
            var query = _clients.Values.AsEnumerable();

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
    }

    public Task SaveAsync(Client client)
    {
        if (client is null)
            throw new ArgumentNullException(nameof(client));

        lock (_lock)
        {
            if (_clients.ContainsKey(client.Id))
                throw new InvalidOperationException($"Client with Id {client.Id} already exists. Use UpdateAsync to modify existing clients.");

            _clients[client.Id] = client;
        }

        return Task.CompletedTask;
    }

    public Task UpdateAsync(Client client)
    {
        if (client is null)
            throw new ArgumentNullException(nameof(client));

        lock (_lock)
        {
            if (!_clients.ContainsKey(client.Id))
                throw new InvalidOperationException($"Client with Id {client.Id} does not exist. Use SaveAsync to add new clients.");

            _clients[client.Id] = client;
        }

        return Task.CompletedTask;
    }
}
