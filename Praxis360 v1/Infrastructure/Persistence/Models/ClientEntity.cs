using System;
using Praxis360.Domain.Types;

namespace Praxis360_v1.Infrastructure.Persistence.Models;

public sealed class ClientEntity
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public Language PreferredLanguage { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Profession { get; set; }
    public string? InamiNumber { get; set; }
}
