using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Praxis360.Domain.Types;
using Praxis360_v1.Domain.Aggregates;
using Praxis360_v1.Domain.Entities;
using Praxis360_v1.Domain.ValueObjects;
using Praxis360_v1.Infrastructure.Persistence.Mappers;
using Xunit;

namespace Praxis360_v1.Tests.Infrastructure.Persistence;

public sealed class PersistenceConstraintTests
{
    [Fact]
    public async Task ExternalReference_DuplicateForSameClient_ShouldViolateUniqueConstraint()
    {
        await using var db = new SqliteTestDatabase();
        await using var context = db.CreateContext();
        await context.Database.EnsureCreatedAsync();

        // Arrange: create client
        var clientId = Guid.NewGuid();
        var client = new Client(
            id: clientId,
            firstName: "John",
            lastName: "Doe",
            dateOfBirth: null,
            preferredLanguage: Language.French
        );
        context.Clients.Add(ClientPersistenceMapper.ToEntity(client));
        await context.SaveChangesAsync();

        // Arrange: create first contract with external reference
        var contract1 = new ContratVie(
            id: Guid.NewGuid(),
            number: new ContractNumber("CONTRACT-001"),
            type: ContractType.IndividualLifeInsurance,
            status: ContractStatus.Active,
            clientId: clientId
        );
        var reference1 = new ExternalReference(
            sourceSystem: SourceSystem.Brio,
            referenceType: ReferenceType.ContractNumber,
            value: "UNIQUE-REF-123"
        );
        contract1.AddExternalReference(reference1);
        context.Contracts.Add(ContractPersistenceMapper.ToEntity(contract1));
        await context.SaveChangesAsync();

        // Arrange: create second contract with SAME external reference for SAME client
        var contract2 = new ContratVie(
            id: Guid.NewGuid(),
            number: new ContractNumber("CONTRACT-002"),
            type: ContractType.IndividualLifeInsurance,
            status: ContractStatus.Active,
            clientId: clientId
        );
        var reference2 = new ExternalReference(
            sourceSystem: SourceSystem.Brio,
            referenceType: ReferenceType.ContractNumber,
            value: "UNIQUE-REF-123"
        );
        contract2.AddExternalReference(reference2);
        context.Contracts.Add(ContractPersistenceMapper.ToEntity(contract2));

        // Act & Assert: unique constraint violation
        var exception = await Assert.ThrowsAsync<DbUpdateException>(async () =>
        {
            await context.SaveChangesAsync();
        });

        Assert.NotNull(exception.InnerException);
        Assert.IsType<SqliteException>(exception.InnerException);
        var sqliteException = (SqliteException)exception.InnerException;
        Assert.Equal(19, sqliteException.SqliteErrorCode); // SQLITE_CONSTRAINT
    }

    [Fact]
    public async Task ExternalReference_SameValueForDifferentClients_ShouldBeAllowed()
    {
        await using var db = new SqliteTestDatabase();
        await using var context = db.CreateContext();
        await context.Database.EnsureCreatedAsync();

        // Arrange: create two clients
        var clientId1 = Guid.NewGuid();
        var client1 = new Client(
            id: clientId1,
            firstName: "Alice",
            lastName: "Smith",
            dateOfBirth: null,
            preferredLanguage: Language.French
        );
        var clientId2 = Guid.NewGuid();
        var client2 = new Client(
            id: clientId2,
            firstName: "Bob",
            lastName: "Jones",
            dateOfBirth: null,
            preferredLanguage: Language.Dutch
        );
        context.Clients.Add(ClientPersistenceMapper.ToEntity(client1));
        context.Clients.Add(ClientPersistenceMapper.ToEntity(client2));
        await context.SaveChangesAsync();

        // Arrange: create contracts with SAME external reference value for DIFFERENT clients
        var contract1 = new ContratVie(
            id: Guid.NewGuid(),
            number: new ContractNumber("CONTRACT-ALICE"),
            type: ContractType.IndividualLifeInsurance,
            status: ContractStatus.Active,
            clientId: clientId1
        );
        var reference1 = new ExternalReference(
            sourceSystem: SourceSystem.Brio,
            referenceType: ReferenceType.ContractNumber,
            value: "SHARED-REF-999"
        );
        contract1.AddExternalReference(reference1);

        var contract2 = new ContratVie(
            id: Guid.NewGuid(),
            number: new ContractNumber("CONTRACT-BOB"),
            type: ContractType.IndividualLifeInsurance,
            status: ContractStatus.Active,
            clientId: clientId2
        );
        var reference2 = new ExternalReference(
            sourceSystem: SourceSystem.Brio,
            referenceType: ReferenceType.ContractNumber,
            value: "SHARED-REF-999"
        );
        contract2.AddExternalReference(reference2);

        context.Contracts.Add(ContractPersistenceMapper.ToEntity(contract1));
        context.Contracts.Add(ContractPersistenceMapper.ToEntity(contract2));

        // Act: should succeed - different clients can have same external reference value
        await context.SaveChangesAsync();

        // Assert
        var savedReferences = await context.ExternalReferences.ToListAsync();
        Assert.Equal(2, savedReferences.Count);
        Assert.All(savedReferences, r => Assert.Equal("SHARED-REF-999", r.Value));
    }

    [Fact]
    public async Task ExternalReference_ClientMismatch_ShouldViolateCompositeForeignKey()
    {
        await using var db = new SqliteTestDatabase();
        await using var context = db.CreateContext();
        await context.Database.EnsureCreatedAsync();

        // Arrange: create two clients
        var clientId1 = Guid.NewGuid();
        var client1 = new Client(
            id: clientId1,
            firstName: "Client",
            lastName: "One",
            dateOfBirth: null,
            preferredLanguage: Language.French
        );
        var clientId2 = Guid.NewGuid();
        var client2 = new Client(
            id: clientId2,
            firstName: "Client",
            lastName: "Two",
            dateOfBirth: null,
            preferredLanguage: Language.French
        );
        context.Clients.Add(ClientPersistenceMapper.ToEntity(client1));
        context.Clients.Add(ClientPersistenceMapper.ToEntity(client2));
        await context.SaveChangesAsync();

        // Arrange: create contract for client1
        var contractId = Guid.NewGuid();
        var contract = new ContratVie(
            id: contractId,
            number: new ContractNumber("CONTRACT-CLIENT1"),
            type: ContractType.IndividualLifeInsurance,
            status: ContractStatus.Active,
            clientId: clientId1
        );
        context.Contracts.Add(ContractPersistenceMapper.ToEntity(contract));
        await context.SaveChangesAsync();

        // Act: attempt to insert external reference with mismatched ClientId
        var mismatchedRef = new Praxis360_v1.Infrastructure.Persistence.Models.ExternalReferenceEntity
        {
            ContractId = contractId,
            ClientId = clientId2, // WRONG: contract belongs to clientId1, not clientId2
            SourceSystem = SourceSystem.Brio,
            ReferenceType = ReferenceType.ContractNumber,
            Value = "MISMATCHED-REF"
        };
        context.ExternalReferences.Add(mismatchedRef);

        // Assert: composite foreign key violation
        var exception = await Assert.ThrowsAsync<DbUpdateException>(async () =>
        {
            await context.SaveChangesAsync();
        });

        Assert.NotNull(exception.InnerException);
        Assert.IsType<SqliteException>(exception.InnerException);
        var sqliteException = (SqliteException)exception.InnerException;
        Assert.Equal(19, sqliteException.SqliteErrorCode); // SQLITE_CONSTRAINT
    }

    [Fact]
    public async Task InitialMigration_ShouldCreateExpectedSchema()
    {
        await using var db = new SqliteTestDatabase();
        await using var context = db.CreateContext();

        // Act: apply migration
        await context.Database.MigrateAsync();

        // Assert: verify tables exist
        var connection = context.Database.GetDbConnection();
        await using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT name FROM sqlite_master 
            WHERE type='table' AND name NOT LIKE 'sqlite_%' AND name NOT LIKE '__EF%'
            ORDER BY name;";

        var tables = new System.Collections.Generic.List<string>();
        await using (var reader = await command.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                tables.Add(reader.GetString(0));
            }
        }

        Assert.Contains("Clients", tables);
        Assert.Contains("Contracts", tables);
        Assert.Contains("ExternalReferences", tables);
        Assert.Contains("ContractProvenances", tables);

        // Assert: verify migration history
        var appliedMigrations = await context.Database.GetAppliedMigrationsAsync();
        Assert.Single(appliedMigrations);
        Assert.Contains("InitialCreate", appliedMigrations.First());

        // Assert: verify key constraints exist (check pragma for Contracts alternate key)
        command.CommandText = "PRAGMA index_list('Contracts');";
        var indexes = new System.Collections.Generic.List<string>();
        await using (var reader = await command.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                indexes.Add(reader.GetString(1));
            }
        }
        // SQLite generates sqlite_autoindex_Contracts_2 for the alternate key
        Assert.Contains(indexes, idx => idx.StartsWith("sqlite_autoindex_Contracts"));

        // Assert: verify unique index on ExternalReferences
        command.CommandText = "PRAGMA index_list('ExternalReferences');";
        var extRefIndexes = new System.Collections.Generic.List<string>();
        await using (var reader = await command.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                extRefIndexes.Add(reader.GetString(1));
            }
        }
        Assert.Contains(extRefIndexes, idx => idx.Contains("ClientId") && idx.Contains("SourceSystem"));
    }
}
