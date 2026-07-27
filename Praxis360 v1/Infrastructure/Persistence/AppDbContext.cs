using Microsoft.EntityFrameworkCore;
using Praxis360_v1.Infrastructure.Persistence.Models;

namespace Praxis360_v1.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    public DbSet<ClientEntity> Clients => Set<ClientEntity>();
    public DbSet<ContractEntity> Contracts => Set<ContractEntity>();
    public DbSet<ExternalReferenceEntity> ExternalReferences => Set<ExternalReferenceEntity>();
    public DbSet<ContractProvenanceEntity> ContractProvenances => Set<ContractProvenanceEntity>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Clients table
        modelBuilder.Entity<ClientEntity>(entity =>
        {
            entity.ToTable("Clients");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.DateOfBirth)
                .IsRequired(false);

            entity.Property(e => e.PreferredLanguage)
                .IsRequired()
                .HasConversion<string>();

            entity.Property(e => e.Email)
                .HasMaxLength(500);

            entity.Property(e => e.Phone)
                .HasMaxLength(50);

            entity.Property(e => e.Profession)
                .HasMaxLength(200);

            entity.Property(e => e.InamiNumber)
                .HasMaxLength(50);
        });

        // Contracts table
        modelBuilder.Entity<ContractEntity>(entity =>
        {
            entity.ToTable("Contracts");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Number)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Type)
                .IsRequired()
                .HasConversion<string>();

            entity.Property(e => e.Status)
                .IsRequired()
                .HasConversion<string>();

            entity.Property(e => e.ClientId)
                .IsRequired();

            // Foreign key to Client
            entity.HasOne(e => e.Client)
                .WithMany()
                .HasForeignKey(e => e.ClientId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // Alternate key (Id, ClientId)
            entity.HasAlternateKey(e => new { e.Id, e.ClientId });
        });

        // ExternalReferences table
        modelBuilder.Entity<ExternalReferenceEntity>(entity =>
        {
            entity.ToTable("ExternalReferences");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.ContractId)
                .IsRequired();

            entity.Property(e => e.ClientId)
                .IsRequired();

            entity.Property(e => e.SourceSystem)
                .IsRequired()
                .HasConversion<string>();

            entity.Property(e => e.ReferenceType)
                .IsRequired()
                .HasConversion<string>();

            entity.Property(e => e.Value)
                .IsRequired()
                .HasMaxLength(500);

            // Composite foreign key to Contracts (Id, ClientId)
            entity.HasOne(e => e.Contract)
                .WithMany(c => c.ExternalReferences)
                .HasForeignKey(e => new { e.ContractId, e.ClientId })
                .HasPrincipalKey(c => new { c.Id, c.ClientId })
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // Unique index for idempotence: (ClientId, SourceSystem, ReferenceType, Value)
            entity.HasIndex(e => new { e.ClientId, e.SourceSystem, e.ReferenceType, e.Value })
                .IsUnique();
        });

        // ContractProvenances table
        modelBuilder.Entity<ContractProvenanceEntity>(entity =>
        {
            entity.ToTable("ContractProvenances");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.ContractId)
                .IsRequired();

            entity.Property(e => e.SourceSystem)
                .IsRequired()
                .HasConversion<string>();

            entity.Property(e => e.RawInsurerName)
                .HasMaxLength(500);

            entity.Property(e => e.ImportedAtUtc)
                .IsRequired();

            entity.Property(e => e.SourceSnapshotDate)
                .IsRequired(false);

            // Foreign key to Contract
            entity.HasOne(e => e.Contract)
                .WithMany(c => c.ContractProvenances)
                .HasForeignKey(e => e.ContractId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });
    }
}
