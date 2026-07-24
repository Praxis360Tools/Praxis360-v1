using System;

namespace Praxis360_v1.Application.Models;

public sealed class SelectableClient
{
    public Guid ClientId { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public DateOnly? DateOfBirth { get; }
    public string DisplayName => $"{FirstName} {LastName}";

    public SelectableClient(Guid clientId, string firstName, string lastName, DateOnly? dateOfBirth)
    {
        if (clientId == Guid.Empty)
            throw new ArgumentException("ClientId must not be empty", nameof(clientId));
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("FirstName must be provided", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("LastName must be provided", nameof(lastName));

        ClientId = clientId;
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        DateOfBirth = dateOfBirth;
    }
}
