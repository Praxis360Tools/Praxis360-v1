using System;
using System.Collections.Generic;
using System.Linq;

namespace Praxis360_v1.Application.Models;

public sealed class BrioContractApplicationResult
{
    public Guid? ClientId { get; }
    public bool ClientWasCreated { get; }
    public IReadOnlyList<ContractCreated> ContractsCreated { get; }
    public IReadOnlyList<ContractAlreadyExisting> ContractsAlreadyExisting { get; }
    public IReadOnlyList<ContractSkipped> ContractsSkipped { get; }
    public IReadOnlyList<ContractUnresolved> ContractsUnresolved { get; }
    public IReadOnlyList<ImportAnalysisIssue> GlobalErrors { get; }
    public IReadOnlyList<ImportAnalysisIssue> GlobalWarnings { get; }
    public ApplicationOutcome Outcome { get; }

    public BrioContractApplicationResult(
        Guid? clientId,
        bool clientWasCreated,
        IReadOnlyList<ContractCreated> contractsCreated,
        IReadOnlyList<ContractAlreadyExisting> contractsAlreadyExisting,
        IReadOnlyList<ContractSkipped> contractsSkipped,
        IReadOnlyList<ContractUnresolved> contractsUnresolved,
        IReadOnlyList<ImportAnalysisIssue> globalErrors,
        IReadOnlyList<ImportAnalysisIssue> globalWarnings,
        ApplicationOutcome outcome)
    {
        ClientId = clientId;
        ClientWasCreated = clientWasCreated;
        ContractsCreated = contractsCreated ?? throw new ArgumentNullException(nameof(contractsCreated));
        ContractsAlreadyExisting = contractsAlreadyExisting ?? throw new ArgumentNullException(nameof(contractsAlreadyExisting));
        ContractsSkipped = contractsSkipped ?? throw new ArgumentNullException(nameof(contractsSkipped));
        ContractsUnresolved = contractsUnresolved ?? throw new ArgumentNullException(nameof(contractsUnresolved));
        GlobalErrors = globalErrors ?? throw new ArgumentNullException(nameof(globalErrors));
        GlobalWarnings = globalWarnings ?? throw new ArgumentNullException(nameof(globalWarnings));
        Outcome = outcome;
    }

    public int TotalContractsProcessed =>
        ContractsCreated.Count +
        ContractsAlreadyExisting.Count +
        ContractsSkipped.Count +
        ContractsUnresolved.Count;

    public int TotalContractsApplied =>
        ContractsCreated.Count +
        ContractsAlreadyExisting.Count;

    public bool HasGlobalErrors => GlobalErrors.Count > 0;

    public bool IsFullSuccess =>
        Outcome == ApplicationOutcome.Success &&
        !HasGlobalErrors &&
        ContractsUnresolved.Count == 0 &&
        ContractsSkipped.Count == 0;
}

public enum ApplicationOutcome
{
    Success,
    PartialSuccess,
    Failed
}

public sealed class ContractCreated
{
    public Guid ContractId { get; }
    public string PolicyNumber { get; }

    public ContractCreated(Guid contractId, string policyNumber)
    {
        ContractId = contractId;
        PolicyNumber = policyNumber ?? throw new ArgumentNullException(nameof(policyNumber));
    }
}

public sealed class ContractAlreadyExisting
{
    public Guid ContractId { get; }
    public string PolicyNumber { get; }

    public ContractAlreadyExisting(Guid contractId, string policyNumber)
    {
        ContractId = contractId;
        PolicyNumber = policyNumber ?? throw new ArgumentNullException(nameof(policyNumber));
    }
}

public sealed class ContractSkipped
{
    public string PolicyNumber { get; }
    public string SkipReason { get; }
    public IReadOnlyList<ImportAnalysisIssue> Issues { get; }

    public ContractSkipped(string policyNumber, string skipReason, IReadOnlyList<ImportAnalysisIssue> issues)
    {
        PolicyNumber = policyNumber ?? throw new ArgumentNullException(nameof(policyNumber));
        SkipReason = skipReason ?? throw new ArgumentNullException(nameof(skipReason));
        Issues = issues ?? throw new ArgumentNullException(nameof(issues));
    }
}

public sealed class ContractUnresolved
{
    public string? PolicyNumber { get; }
    public string UnresolvedReason { get; }
    public IReadOnlyList<ImportAnalysisIssue> Issues { get; }

    public ContractUnresolved(string? policyNumber, string unresolvedReason, IReadOnlyList<ImportAnalysisIssue> issues)
    {
        PolicyNumber = policyNumber;
        UnresolvedReason = unresolvedReason ?? throw new ArgumentNullException(nameof(unresolvedReason));
        Issues = issues ?? throw new ArgumentNullException(nameof(issues));
    }
}
