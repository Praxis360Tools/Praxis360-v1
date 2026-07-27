using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Praxis360.Domain.Types;
using Praxis360_v1.Domain.Aggregates;
using Praxis360_v1.Domain.Entities;
using Praxis360_v1.Domain.ValueObjects;
using Praxis360_v1.Infrastructure.Persistence.Mappers;
using Xunit;

namespace Praxis360_v1.Tests.Infrastructure.Persistence;

public sealed class PersistenceMappingTests
{
    [Fact]
    public async Task Client_RoundTrip_ShouldPreservePersistedProperties()
    {
        await using var db = new SqliteTestDatabase();
        await using var context = db.CreateContext();
        await context.Database.EnsureCreatedAsync();

        // Arrange
        var clientId = Guid.NewGuid();
        var dateOfBirth = new DateOnly(1985, 6, 15);
        var originalClient = new Client(
            id: clientId,
            firstName: "Jean",
            lastName: "Dupont",
            dateOfBirth: dateOfBirth,
            preferredLanguage: Language.French
        );
        originalClient.UpdateContactAndProfessionalInfo(
            email: "jean.dupont@example.com",
            phone: "+32 2 123 45 67",
            profession: "Médecin",
            inamiNumber: "12345678901"
        );

        // Act: persist
        var entity = ClientPersistenceMapper.ToEntity(originalClient);
        context.Clients.Add(entity);
        await context.SaveChangesAsync();

        // Act: reload
        var reloadedEntity = await context.Clients.AsNoTracking().FirstAsync(c => c.Id == clientId);
        var rehydratedClient = ClientPersistenceMapper.ToDomain(reloadedEntity);

        // Assert
        Assert.Equal(originalClient.Id, rehydratedClient.Id);
        Assert.Equal(originalClient.FirstName, rehydratedClient.FirstName);
        Assert.Equal(originalClient.LastName, rehydratedClient.LastName);
        Assert.Equal(originalClient.DateOfBirth, rehydratedClient.DateOfBirth);
        Assert.Equal(originalClient.PreferredLanguage, rehydratedClient.PreferredLanguage);
        Assert.Equal(originalClient.Email, rehydratedClient.Email);
        Assert.Equal(originalClient.Phone, rehydratedClient.Phone);
        Assert.Equal(originalClient.Profession, rehydratedClient.Profession);
        Assert.Equal(originalClient.InamiNumber, rehydratedClient.InamiNumber);
    }

    [Fact]
    public async Task Client_RoundTrip_WithNullDateOfBirth_ShouldPreserveNull()
    {
        await using var db = new SqliteTestDatabase();
        await using var context = db.CreateContext();
        await context.Database.EnsureCreatedAsync();

        // Arrange
        var clientId = Guid.NewGuid();
        var originalClient = new Client(
            id: clientId,
            firstName: "Marie",
            lastName: "Martin",
            dateOfBirth: null,
            preferredLanguage: Language.Dutch
        );

        // Act: persist
        var entity = ClientPersistenceMapper.ToEntity(originalClient);
        context.Clients.Add(entity);
        await context.SaveChangesAsync();

        // Act: reload
        var reloadedEntity = await context.Clients.AsNoTracking().FirstAsync(c => c.Id == clientId);
        var rehydratedClient = ClientPersistenceMapper.ToDomain(reloadedEntity);

        // Assert
        Assert.Null(rehydratedClient.DateOfBirth);
        Assert.Equal(originalClient.Id, rehydratedClient.Id);
        Assert.Equal(originalClient.FirstName, rehydratedClient.FirstName);
        Assert.Equal(originalClient.LastName, rehydratedClient.LastName);
        Assert.Equal(originalClient.PreferredLanguage, rehydratedClient.PreferredLanguage);
    }

    [Fact]
    public async Task Contract_RoundTrip_ShouldPreserveExternalReferencesAndProvenances()
    {
        await using var db = new SqliteTestDatabase();
        await using var context = db.CreateContext();
        await context.Database.EnsureCreatedAsync();

        // Arrange: create client first
        var clientId = Guid.NewGuid();
        var client = new Client(
            id: clientId,
            firstName: "Test",
            lastName: "Client",
            dateOfBirth: null,
            preferredLanguage: Language.French
        );
        var clientEntity = ClientPersistenceMapper.ToEntity(client);
        context.Clients.Add(clientEntity);
        await context.SaveChangesAsync();

        // Arrange: create contract with references and provenances
        var contractId = Guid.NewGuid();
        var originalContract = new ContratVie(
            id: contractId,
            number: new ContractNumber("BRIO-12345"),
            type: ContractType.IndividualLifeInsurance,
            status: ContractStatus.Active,
            clientId: clientId
        );

        var reference = new ExternalReference(
            sourceSystem: SourceSystem.Brio,
            referenceType: ReferenceType.ContractNumber,
            value: "BRIO-EXT-12345"
        );
        originalContract.AddExternalReference(reference);

        var provenance = new ContractProvenance(
            sourceSystem: SourceSystem.Brio,
            importedAtUtc: new DateTime(2026, 1, 15, 10, 30, 0, DateTimeKind.Utc),
            rawInsurerName: "AG Insurance",
            sourceSnapshotDate: new DateOnly(2026, 1, 14)
        );
        originalContract.AddProvenance(provenance);

        // Act: persist
        var contractEntity = ContractPersistenceMapper.ToEntity(originalContract);
        context.Contracts.Add(contractEntity);
        await context.SaveChangesAsync();

        // Act: reload
        var reloadedEntity = await context.Contracts
            .AsNoTracking()
            .Include(c => c.ExternalReferences)
            .Include(c => c.ContractProvenances)
            .FirstAsync(c => c.Id == contractId);
        var rehydratedContract = ContractPersistenceMapper.ToDomain(reloadedEntity);

        // Assert
        Assert.Equal(originalContract.Id, rehydratedContract.Id);
        Assert.Equal(originalContract.Number.Value, rehydratedContract.Number.Value);
        Assert.Equal(originalContract.Type, rehydratedContract.Type);
        Assert.Equal(originalContract.Status, rehydratedContract.Status);
        Assert.Equal(originalContract.ClientId, rehydratedContract.ClientId);

        Assert.Single(rehydratedContract.ExternalReferences);
        var rehydratedRef = rehydratedContract.ExternalReferences.First();
        Assert.Equal(reference.SourceSystem, rehydratedRef.SourceSystem);
        Assert.Equal(reference.ReferenceType, rehydratedRef.ReferenceType);
        Assert.Equal(reference.Value, rehydratedRef.Value);

        Assert.Single(rehydratedContract.Provenances);
        var rehydratedProv = rehydratedContract.Provenances.First();
        Assert.Equal(provenance.SourceSystem, rehydratedProv.SourceSystem);
        Assert.Equal(provenance.ImportedAtUtc, rehydratedProv.ImportedAtUtc);
        Assert.Equal(provenance.RawInsurerName, rehydratedProv.RawInsurerName);
        Assert.Equal(provenance.SourceSnapshotDate, rehydratedProv.SourceSnapshotDate);
    }

    [Fact]
    public async Task Persistence_BetweenTwoDbContexts_ShouldBePreserved()
    {
        await using var db = new SqliteTestDatabase();

        var clientId = Guid.NewGuid();
        var contractId = Guid.NewGuid();

        // Act: persist with first context
        await using (var context1 = db.CreateContext())
        {
            await context1.Database.EnsureCreatedAsync();

            var client = new Client(
                id: clientId,
                firstName: "Alice",
                lastName: "Wonderland",
                dateOfBirth: new DateOnly(1990, 3, 21),
                preferredLanguage: Language.English
            );

            var contract = new ContratVie(
                id: contractId,
                number: new ContractNumber("PERSISTENT-001"),
                type: ContractType.IndividualLifeInsurance,
                status: ContractStatus.Active,
                clientId: clientId
            );

            context1.Clients.Add(ClientPersistenceMapper.ToEntity(client));
            context1.Contracts.Add(ContractPersistenceMapper.ToEntity(contract));
            await context1.SaveChangesAsync();
        }

        // Act: reload with second context
        await using (var context2 = db.CreateContext())
        {
            var reloadedClient = await context2.Clients.AsNoTracking().FirstOrDefaultAsync(c => c.Id == clientId);
            var reloadedContract = await context2.Contracts.AsNoTracking().FirstOrDefaultAsync(c => c.Id == contractId);

            // Assert
            Assert.NotNull(reloadedClient);
            Assert.Equal("Alice", reloadedClient.FirstName);
            Assert.Equal("Wonderland", reloadedClient.LastName);

            Assert.NotNull(reloadedContract);
            Assert.Equal("PERSISTENT-001", reloadedContract.Number);
            Assert.Equal(clientId, reloadedContract.ClientId);
        }
    }
}
