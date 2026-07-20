using System;
using Praxis360.Domain.Types;
using Praxis360_v1.Domain.ValueObjects;

namespace Praxis360_v1.Domain.Entities;

public sealed class Client
{
    public Guid Id { get; }

    // Identity fields separated: first name and last name
    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public DateOnly? DateOfBirth { get; private set; }

    public Language PreferredLanguage { get; private set; }

    public Adresse? Address { get; private set; }

    public Client(
        Guid id,
        string firstName,
        string lastName,
        DateOnly? dateOfBirth,
        Language preferredLanguage,
        Adresse? address = null)
    {
        Id = id == Guid.Empty ? throw new ArgumentException("Id must be provided.", nameof(id)) : id;

        FirstName = string.IsNullOrWhiteSpace(firstName) ? throw new ArgumentException("FirstName must be provided.", nameof(firstName)) : firstName.Trim();

        LastName = string.IsNullOrWhiteSpace(lastName) ? throw new ArgumentException("LastName must be provided.", nameof(lastName)) : lastName.Trim();

        if (dateOfBirth.HasValue)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (dateOfBirth.Value > today)
                throw new ArgumentException("DateOfBirth cannot be in the future.", nameof(dateOfBirth));
        }

        DateOfBirth = dateOfBirth;

        PreferredLanguage = preferredLanguage;

        Address = address;
    }

    public void UpdateAddress(Adresse? address)
    {
        Address = address;
    }
}
