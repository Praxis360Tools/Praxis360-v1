using System;
using System.Collections.Generic;
using System.Linq;
using Praxis360.Domain.Types;
using Praxis360_v1.Domain.Entities;
using Praxis360_v1.Domain.ValueObjects;

namespace Praxis360_v1.Domain.Aggregates;

public sealed class ContratVie
{
    public Guid Id { get; }

    public ContractNumber Number { get; }

    public ContractType Type { get; }

    public ContractStatus Status { get; private set; }

    public DateRange? Period { get; private set; }

    public Insurer? Insurer { get; }

    public IReadOnlyCollection<Garantie> Garanties => _garanties.AsReadOnly();

    public IReadOnlyCollection<Beneficiary> Beneficiaries => _beneficiaries.AsReadOnly();

    private readonly List<Garantie> _garanties = new();

    private readonly List<Beneficiary> _beneficiaries = new();

    private readonly List<ExternalReference> _externalReferences = new();

    private readonly List<ContractProvenance> _provenances = new();

    // Relation to Client is explicit: ClientId references the Client.Id in Praxis360
    public Guid ClientId { get; }

    public Policyholder? Policyholder { get; private set; }

    public IReadOnlyCollection<ExternalReference> ExternalReferences => _externalReferences.AsReadOnly();

    public IReadOnlyCollection<ContractProvenance> Provenances => _provenances.AsReadOnly();

    public ContratVie(Guid id, ContractNumber number, ContractType type, ContractStatus status, Guid clientId, Insurer? insurer = null)
    {
        Id = id == Guid.Empty ? throw new ArgumentException("Id must be provided.", nameof(id)) : id;

        Number = number ?? throw new ArgumentNullException(nameof(number));

        Type = type;

        Status = status;

        ClientId = clientId == Guid.Empty ? throw new ArgumentException("ClientId must be provided.", nameof(clientId)) : clientId;

        Insurer = insurer;
    }

    public void SetPeriod(DateRange? period)
    {
        Period = period;
    }

    public void AddGarantie(Garantie garantie)
    {
        if (garantie is null)
            throw new ArgumentNullException(nameof(garantie));

        // Prevent duplicate garanties by Id
        if (_garanties.Any(g => g.Id == garantie.Id))
            throw new InvalidOperationException("Garantie with same Id already exists on contract.");

        _garanties.Add(garantie);
    }

    public void AddBeneficiary(Beneficiary beneficiary)
    {
        if (beneficiary is null)
            throw new ArgumentNullException(nameof(beneficiary));

        // Prevent duplicate beneficiaries by Id
        if (_beneficiaries.Any(b => b.Id == beneficiary.Id))
            throw new InvalidOperationException("Beneficiary with same Id already exists on contract.");

        // Calculate projected total including the new beneficiary
        decimal current = _beneficiaries.Sum(b => b.Share.Fraction);
        decimal projected = current + beneficiary.Share.Fraction;

        if (projected > 1m)
            throw new InvalidOperationException("Adding this beneficiary would exceed 100% total shares.");

        _beneficiaries.Add(beneficiary);
    }

    public void UpdateBeneficiaryShare(Guid beneficiaryId, Percentage newShare)
    {
        if (newShare is null)
            throw new ArgumentNullException(nameof(newShare));

        var beneficiary = _beneficiaries.FirstOrDefault(b => b.Id == beneficiaryId);

        if (beneficiary is null)
            throw new InvalidOperationException("Beneficiary not found on contract.");

        // Compute projected total after update
        decimal totalWithout = _beneficiaries.Where(b => b.Id != beneficiaryId).Sum(b => b.Share.Fraction);
        decimal projected = totalWithout + newShare.Fraction;

        if (projected > 1m)
            throw new InvalidOperationException("Updating this beneficiary would exceed 100% total shares.");

        // Only aggregate root can modify the beneficiary share
        beneficiary.UpdateShareInternal(newShare);
    }

    public void AddExternalReference(ExternalReference reference)
    {
        if (reference is null)
            throw new ArgumentNullException(nameof(reference));

        // Prevent duplicate references (same source, type and value)
        if (_externalReferences.Any(r =>
            r.SourceSystem == reference.SourceSystem &&
            r.ReferenceType == reference.ReferenceType &&
            r.Value == reference.Value))
        {
            return; // Idempotent: reference already exists
        }

        _externalReferences.Add(reference);
    }

    public void AddProvenance(ContractProvenance provenance)
    {
        if (provenance is null)
            throw new ArgumentNullException(nameof(provenance));

        // Prevent duplicate provenances (strict structural equality)
        if (_provenances.Any(p => p.Equals(provenance)))
        {
            return; // Idempotent: provenance already exists
        }

        _provenances.Add(provenance);
    }

    public void SetPolicyholder(Policyholder? policyholder)
    {
        Policyholder = policyholder;
    }
}

