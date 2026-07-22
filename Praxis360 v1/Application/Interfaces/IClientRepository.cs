using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Praxis360_v1.Domain.Entities;

namespace Praxis360_v1.Application.Interfaces;

public interface IClientRepository
{
    Task<Client?> GetByIdAsync(Guid id);
    Task<IReadOnlyCollection<Client>> GetAllAsync();
    Task<IReadOnlyCollection<Client>> SearchByIdentityAsync(string firstName, string lastName, DateOnly? dateOfBirth);
    Task SaveAsync(Client client);
    Task UpdateAsync(Client client);
}
