using System;
using System.Linq;
using Praxis360_v1.Domain.Aggregates;
using Praxis360_v1.Domain.ValueObjects;
using Praxis360_v1.Infrastructure.Persistence.Models;

namespace Praxis360_v1.Infrastructure.Persistence.Mappers;

public static class ContractPersistenceMapper
{
    public static ContractEntity ToEntity(ContratVie domain)
    {
        if (domain == null)
            throw new ArgumentNullException(nameof(domain));

        var entity = new ContractEntity
        {
            Id = domain.Id,
            Number = domain.Number.Value,
            Type = domain.Type,
            Status = domain.Status,
            ClientId = domain.ClientId,
            ExternalReferences = domain.ExternalReferences
                .Select(r => new ExternalReferenceEntity
                {
                    ContractId = domain.Id,
                    ClientId = domain.ClientId,
                    SourceSystem = r.SourceSystem,
                    ReferenceType = r.ReferenceType,
                    Value = r.Value
                })
                .ToList(),
            ContractProvenances = domain.Provenances
                .Select(p => new ContractProvenanceEntity
                {
                    ContractId = domain.Id,
                    SourceSystem = p.SourceSystem,
                    RawInsurerName = p.RawInsurerName,
                    ImportedAtUtc = p.ImportedAtUtc,
                    SourceSnapshotDate = p.SourceSnapshotDate
                })
                .ToList()
        };

        return entity;
    }

    public static ContratVie ToDomain(ContractEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var contract = new ContratVie(
            id: entity.Id,
            number: new ContractNumber(entity.Number),
            type: entity.Type,
            status: entity.Status,
            clientId: entity.ClientId,
            insurer: null
        );

        foreach (var refEntity in entity.ExternalReferences)
        {
            var reference = new ExternalReference(
                sourceSystem: refEntity.SourceSystem,
                referenceType: refEntity.ReferenceType,
                value: refEntity.Value
            );
            contract.AddExternalReference(reference);
        }

        foreach (var provEntity in entity.ContractProvenances)
        {
            var importedAtUtc = provEntity.ImportedAtUtc.Kind == DateTimeKind.Utc
                ? provEntity.ImportedAtUtc
                : DateTime.SpecifyKind(provEntity.ImportedAtUtc, DateTimeKind.Utc);

            var provenance = new ContractProvenance(
                sourceSystem: provEntity.SourceSystem,
                importedAtUtc: importedAtUtc,
                rawInsurerName: provEntity.RawInsurerName,
                sourceSnapshotDate: provEntity.SourceSnapshotDate
            );
            contract.AddProvenance(provenance);
        }

        return contract;
    }
}
