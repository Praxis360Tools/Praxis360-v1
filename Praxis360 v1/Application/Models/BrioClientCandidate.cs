using System;

namespace Praxis360_v1.Application.Models;

public sealed class BrioClientCandidate
{
    public string NormalizedIdentity { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public DateOnly? DateOfBirth { get; private set; }
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public string? Profession { get; private set; }
    public string? InamiNumber { get; private set; }

    public BrioClientCandidate(string normalizedIdentity, string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(normalizedIdentity))
            throw new ArgumentException("NormalizedIdentity must be provided", nameof(normalizedIdentity));
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("FirstName must be provided", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("LastName must be provided", nameof(lastName));

        NormalizedIdentity = normalizedIdentity;
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
    }

    public void SetDateOfBirth(DateOnly? dateOfBirth)
    {
        DateOfBirth = dateOfBirth;
    }

    public void SetContactInfo(string? email, string? phone)
    {
        Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
        Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
    }

    public void SetProfessionalInfo(string? profession, string? inamiNumber)
    {
        Profession = string.IsNullOrWhiteSpace(profession) ? null : profession.Trim();
        InamiNumber = string.IsNullOrWhiteSpace(inamiNumber) ? null : inamiNumber.Trim();
    }
}
