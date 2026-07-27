using System;
using Praxis360_v1.Domain.Entities;
using Praxis360_v1.Infrastructure.Persistence.Models;

namespace Praxis360_v1.Infrastructure.Persistence.Mappers;

public static class ClientPersistenceMapper
{
    public static ClientEntity ToEntity(Client domain)
    {
        if (domain == null)
            throw new ArgumentNullException(nameof(domain));

        return new ClientEntity
        {
            Id = domain.Id,
            FirstName = domain.FirstName,
            LastName = domain.LastName,
            DateOfBirth = domain.DateOfBirth,
            PreferredLanguage = domain.PreferredLanguage,
            Email = domain.Email,
            Phone = domain.Phone,
            Profession = domain.Profession,
            InamiNumber = domain.InamiNumber
        };
    }

    public static Client ToDomain(ClientEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var client = new Client(
            id: entity.Id,
            firstName: entity.FirstName,
            lastName: entity.LastName,
            dateOfBirth: entity.DateOfBirth,
            preferredLanguage: entity.PreferredLanguage,
            address: null
        );

        if (!string.IsNullOrWhiteSpace(entity.Email) ||
            !string.IsNullOrWhiteSpace(entity.Phone) ||
            !string.IsNullOrWhiteSpace(entity.Profession) ||
            !string.IsNullOrWhiteSpace(entity.InamiNumber))
        {
            client.UpdateContactAndProfessionalInfo(
                email: entity.Email,
                phone: entity.Phone,
                profession: entity.Profession,
                inamiNumber: entity.InamiNumber
            );
        }

        return client;
    }
}
