using Microsoft.EntityFrameworkCore;
using Praxis360.Domain.Types;
using Praxis360_v1.Application.Interfaces;
using Praxis360_v1.Domain.Aggregates;
using Praxis360_v1.Infrastructure.Persistence;
using Praxis360_v1.Infrastructure.Persistence.Mappers;

namespace Praxis360_v1.Infrastructure.Repositories;

/// <summary>
/// Repository EF Core pour les contrats d'assurance vie.
/// </summary>
public sealed class EfCoreContractRepository : IContractRepository
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public EfCoreContractRepository(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    public async Task<ContratVie?> GetByIdAsync(Guid id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var entity = await context.Contracts
            .AsNoTracking()
            .Include(c => c.ExternalReferences)
            .Include(c => c.ContractProvenances)
            .FirstOrDefaultAsync(c => c.Id == id);

        return entity is null ? null : ContractPersistenceMapper.ToDomain(entity);
    }

    public async Task<IReadOnlyCollection<ContratVie>> GetByClientIdAsync(Guid clientId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var entities = await context.Contracts
            .AsNoTracking()
            .Include(c => c.ExternalReferences)
            .Include(c => c.ContractProvenances)
            .Where(c => c.ClientId == clientId)
            .ToListAsync();

        return entities.Select(ContractPersistenceMapper.ToDomain).ToList();
    }

    public async Task<ContratVie?> FindByExternalReferenceAsync(
        Guid clientId,
        SourceSystem sourceSystem,
        ReferenceType referenceType,
        string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Reference value must be provided", nameof(value));

        await using var context = await _contextFactory.CreateDbContextAsync();
        var searchValue = value.Trim();

        var entity = await context.Contracts
            .AsNoTracking()
            .Include(c => c.ExternalReferences)
            .Include(c => c.ContractProvenances)
            .Where(c => c.ClientId == clientId)
            .Where(c => c.ExternalReferences.Any(r =>
                r.SourceSystem == sourceSystem &&
                r.ReferenceType == referenceType &&
                r.Value == searchValue))
            .FirstOrDefaultAsync();

        return entity is null ? null : ContractPersistenceMapper.ToDomain(entity);
    }

    public async Task SaveAsync(ContratVie contract)
    {
        if (contract is null)
            throw new ArgumentNullException(nameof(contract));

        await using var context = await _contextFactory.CreateDbContextAsync();
        var entity = ContractPersistenceMapper.ToEntity(contract);
        context.Contracts.Add(entity);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ContratVie contract)
    {
        if (contract is null)
            throw new ArgumentNullException(nameof(contract));

        await using var context = await _contextFactory.CreateDbContextAsync();
        var existing = await context.Contracts
            .FirstOrDefaultAsync(c => c.Id == contract.Id && c.ClientId == contract.ClientId);
        if (existing is null)
            throw new InvalidOperationException($"Contract with Id {contract.Id} not found.");

        existing.Number = contract.Number.Value;
        existing.Type = contract.Type;
        existing.Status = contract.Status;
        await context.SaveChangesAsync();
    }
}
