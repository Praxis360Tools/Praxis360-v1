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

    // Optional contact and professional information
    public string? Email { get; private set; }

    public string? Phone { get; private set; }

    public string? Profession { get; private set; }

    public string? InamiNumber { get; private set; }

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

    public void UpdateContactAndProfessionalInfo(
        string? email = null,
        string? phone = null,
        string? profession = null,
        string? inamiNumber = null)
    {
        Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
        Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
        Profession = string.IsNullOrWhiteSpace(profession) ? null : profession.Trim();
        InamiNumber = string.IsNullOrWhiteSpace(inamiNumber) ? null : inamiNumber.Trim();
    }
}
