using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Praxis360.Domain.Types;
using Praxis360_v1.Application.Constants;
using Praxis360_v1.Application.Interfaces;
using Praxis360_v1.Application.Models;
using Praxis360_v1.Domain.Aggregates;
using Praxis360_v1.Domain.Entities;
using Praxis360_v1.Domain.ValueObjects;

namespace Praxis360_v1.Application.Services;

public sealed class BrioContractApplicationService : IBrioContractApplicationService
{
    private readonly IClientRepository _clientRepository;
    private readonly IContractRepository _contractRepository;

    public BrioContractApplicationService(
        IClientRepository clientRepository,
        IContractRepository contractRepository)
    {
        _clientRepository = clientRepository ?? throw new ArgumentNullException(nameof(clientRepository));
        _contractRepository = contractRepository ?? throw new ArgumentNullException(nameof(contractRepository));
    }

    public async Task<BrioContractApplicationResult> ApplyToExistingClientAsync(
        BrioImportAnalysisResult analysisResult,
        string selectedClientNormalizedIdentity,
        Guid existingClientId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Pre-validation
        if (analysisResult is null)
            return FailedResult(null, "BRIO_ANALYSIS_NULL", "Analysis result must be provided");

        if (string.IsNullOrWhiteSpace(selectedClientNormalizedIdentity))
            return FailedResult(null, "BRIO_CLIENT_IDENTITY_EMPTY", "Selected client normalized identity must be provided");

        if (existingClientId == Guid.Empty)
            return FailedResult(null, "PRAXIS_CLIENT_ID_INVALID", "Existing client ID must be provided");

        // Resolve BRIO client candidate
        if (!analysisResult.ClientCandidates.TryGetValue(selectedClientNormalizedIdentity, out var brioClientCandidate))
            return FailedResult(null, "BRIO_CLIENT_CANDIDATE_NOT_FOUND", "The selected BRIO client candidate was not found in the analysis result");

        // Resolve existing Praxis360 client
        var existingClient = await _clientRepository.GetByIdAsync(existingClientId);
        if (existingClient is null)
            return FailedResult(null, "PRAXIS_CLIENT_NOT_FOUND", $"Praxis360 client not found with ID: {existingClientId}");

        // Select contract candidates for this identity
        var contractCandidates = analysisResult.ContractCandidates
            .Where(c => c.ClientNormalizedIdentity == selectedClientNormalizedIdentity)
            .ToList();

        if (contractCandidates.Count == 0)
            return FailedResult(existingClientId, "BRIO_NO_CONTRACT_CANDIDATES", "No contract candidates found for the selected client identity");

        cancellationToken.ThrowIfCancellationRequested();

        // Process contracts
        var result = await ProcessContractsAsync(
            existingClient,
            contractCandidates,
            analysisResult.AnalyzedLines,
            clientWasCreated: false,
            cancellationToken);

        return result;
    }

    public async Task<BrioContractApplicationResult> ApplyWithNewClientAsync(
        BrioImportAnalysisResult analysisResult,
        string selectedClientNormalizedIdentity,
        Language preferredLanguage,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Pre-validation
        if (analysisResult is null)
            return FailedResult(null, "BRIO_ANALYSIS_NULL", "Analysis result must be provided");

        if (string.IsNullOrWhiteSpace(selectedClientNormalizedIdentity))
            return FailedResult(null, "BRIO_CLIENT_IDENTITY_EMPTY", "Selected client normalized identity must be provided");

        if (!Enum.IsDefined(typeof(Language), preferredLanguage))
            return FailedResult(null, "PRAXIS_LANGUAGE_INVALID", $"Preferred language is not valid: {preferredLanguage}");

        // Resolve BRIO client candidate
        if (!analysisResult.ClientCandidates.TryGetValue(selectedClientNormalizedIdentity, out var brioClientCandidate))
            return FailedResult(null, "BRIO_CLIENT_CANDIDATE_NOT_FOUND", "The selected BRIO client candidate was not found in the analysis result");

        // Select contract candidates for this identity
        var contractCandidates = analysisResult.ContractCandidates
            .Where(c => c.ClientNormalizedIdentity == selectedClientNormalizedIdentity)
            .ToList();

        if (contractCandidates.Count == 0)
            return FailedResult(null, "BRIO_NO_CONTRACT_CANDIDATES", "No contract candidates found for the selected client identity");

        // Build new client in memory (not saved yet)
        var newClientId = Guid.NewGuid();
        var newClient = new Client(
            newClientId,
            brioClientCandidate.FirstName,
            brioClientCandidate.LastName,
            brioClientCandidate.DateOfBirth,
            preferredLanguage,
            address: null);

        newClient.UpdateContactAndProfessionalInfo(
            brioClientCandidate.Email,
            brioClientCandidate.Phone,
            brioClientCandidate.Profession,
            brioClientCandidate.InamiNumber);

        cancellationToken.ThrowIfCancellationRequested();

        // Process contracts
        var result = await ProcessContractsAsync(
            newClient,
            contractCandidates,
            analysisResult.AnalyzedLines,
            clientWasCreated: true,
            cancellationToken);

        return result;
    }

    private async Task<BrioContractApplicationResult> ProcessContractsAsync(
        Client client,
        List<BrioContractCandidate> contractCandidates,
        IReadOnlyList<BrioAnalyzedLine> analyzedLines,
        bool clientWasCreated,
        CancellationToken cancellationToken)
    {
        var contractsToCreate = new List<ContratVie>();
        var contractsCreatedResult = new List<ContractCreated>();
        var contractsAlreadyExistingResult = new List<ContractAlreadyExisting>();
        var contractsSkippedResult = new List<ContractSkipped>();
        var contractsUnresolvedResult = new List<ContractUnresolved>();
        var globalWarnings = new List<ImportAnalysisIssue>();

        foreach (var candidate in contractCandidates)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Retrieve source lines
            var sourceLines = candidate.SourceLineNumbers
                .Select(lineNum => analyzedLines.FirstOrDefault(l => l.LineNumber == lineNum))
                .ToList();

            var missingLines = candidate.SourceLineNumbers
                .Where(lineNum => !analyzedLines.Any(l => l.LineNumber == lineNum))
                .ToList();

            var candidateIssues = new List<ImportAnalysisIssue>();

            // Check for missing source lines
            if (missingLines.Count > 0)
            {
                var unresolvedIssue = new ImportAnalysisIssue(
                    "BRIO_SOURCE_LINE_NOT_FOUND",
                    ImportIssueSeverity.BlockingError,
                    $"Source line(s) not found: {string.Join(", ", missingLines)}",
                    lineNumber: null,
                    fieldName: null);
                candidateIssues.Add(unresolvedIssue);

                contractsUnresolvedResult.Add(new ContractUnresolved(
                    candidate.NormalizedPolicyNumber,
                    "Source line(s) not found in analysis result",
                    candidateIssues));
                continue;
            }

            // Aggregate issues from source lines
            foreach (var sourceLine in sourceLines.Where(l => l is not null))
            {
                candidateIssues.AddRange(sourceLine!.Issues);
            }

            // Check for blocking errors in source lines
            var hasBlockingErrors = sourceLines.Any(l => l is not null && l.HasBlockingErrors);
            if (hasBlockingErrors)
            {
                var blockingIssue = new ImportAnalysisIssue(
                    "BRIO_SOURCE_LINE_BLOCKED",
                    ImportIssueSeverity.BlockingError,
                    "One or more source lines contain blocking errors",
                    lineNumber: null,
                    fieldName: null);
                candidateIssues.Add(blockingIssue);

                contractsUnresolvedResult.Add(new ContractUnresolved(
                    candidate.NormalizedPolicyNumber,
                    "Source line(s) contain blocking errors",
                    candidateIssues));
                continue;
            }

            // Resolve ContractType
            if (!candidate.MappedType.HasValue)
            {
                var unresolvedIssue = new ImportAnalysisIssue(
                    "BRIO_PRODUCT_UNRESOLVED",
                    ImportIssueSeverity.BlockingError,
                    $"Product code could not be mapped: {candidate.RawProductCode}",
                    lineNumber: null,
                    fieldName: "ProductCode");
                candidateIssues.Add(unresolvedIssue);

                contractsUnresolvedResult.Add(new ContractUnresolved(
                    candidate.NormalizedPolicyNumber,
                    "Product code could not be resolved to a ContractType",
                    candidateIssues));
                continue;
            }

            // Resolve ContractStatus
            var mappedStatus = BrioStatusMapping.MapStatus(candidate.RawStatus);
            if (!mappedStatus.HasValue)
            {
                var unresolvedIssue = new ImportAnalysisIssue(
                    "BRIO_STATUS_UNRESOLVED",
                    ImportIssueSeverity.BlockingError,
                    $"Status could not be mapped: {candidate.RawStatus ?? "(empty)"}",
                    lineNumber: null,
                    fieldName: "Status");
                candidateIssues.Add(unresolvedIssue);

                contractsUnresolvedResult.Add(new ContractUnresolved(
                    candidate.NormalizedPolicyNumber,
                    "Status could not be resolved to a ContractStatus",
                    candidateIssues));
                continue;
            }

            // Check idempotence
            var existingContract = await _contractRepository.FindByExternalReferenceAsync(
                client.Id,
                SourceSystem.Brio,
                ReferenceType.PolicyNumber,
                candidate.NormalizedPolicyNumber);

            if (existingContract is not null)
            {
                contractsAlreadyExistingResult.Add(new ContractAlreadyExisting(
                    existingContract.Id,
                    candidate.NormalizedPolicyNumber));
                continue;
            }

            // Build contract in memory
            var contractId = Guid.NewGuid();
            var contractNumber = new ContractNumber(candidate.NormalizedPolicyNumber);
            var contract = new ContratVie(
                contractId,
                contractNumber,
                candidate.MappedType.Value,
                mappedStatus.Value,
                client.Id,
                insurer: null);

            // Add external reference
            var externalReference = new ExternalReference(
                SourceSystem.Brio,
                ReferenceType.PolicyNumber,
                candidate.NormalizedPolicyNumber);
            contract.AddExternalReference(externalReference);

            // Add provenance
            var provenance = new ContractProvenance(
                SourceSystem.Brio,
                DateTime.UtcNow,
                candidate.RawInsurerName,
                sourceSnapshotDate: null);
            contract.AddProvenance(provenance);

            contractsToCreate.Add(contract);
            contractsCreatedResult.Add(new ContractCreated(contractId, candidate.NormalizedPolicyNumber));
        }

        cancellationToken.ThrowIfCancellationRequested();

        // Determine outcome before any writes
        if (contractsToCreate.Count == 0)
        {
            if (clientWasCreated && contractsUnresolvedResult.Count > 0)
            {
                // New client with no creatable contracts - do not save client
                var noCreatableError = new ImportAnalysisIssue(
                    "BRIO_NO_CREATABLE_CONTRACTS",
                    ImportIssueSeverity.BlockingError,
                    "No contracts could be created for the new client",
                    lineNumber: null,
                    fieldName: null);

                return new BrioContractApplicationResult(
                    clientId: null,
                    clientWasCreated: false,
                    contractsCreated: new List<ContractCreated>(),
                    contractsAlreadyExisting: new List<ContractAlreadyExisting>(),
                    contractsSkipped: new List<ContractSkipped>(),
                    contractsUnresolved: contractsUnresolvedResult,
                    globalErrors: new List<ImportAnalysisIssue> { noCreatableError },
                    globalWarnings: globalWarnings,
                    outcome: ApplicationOutcome.Failed);
            }
            else
            {
                // No new contracts to create - use DetermineOutcome to decide
                var finalOutcome = DetermineOutcome(
                    contractsCreatedResult,
                    contractsAlreadyExistingResult,
                    contractsSkippedResult,
                    contractsUnresolvedResult,
                    globalErrors: new List<ImportAnalysisIssue>());

                return new BrioContractApplicationResult(
                    client.Id,
                    clientWasCreated: false,
                    contractsCreatedResult,
                    contractsAlreadyExistingResult,
                    contractsSkippedResult,
                    contractsUnresolvedResult,
                    globalErrors: new List<ImportAnalysisIssue>(),
                    globalWarnings,
                    finalOutcome);
            }
        }

        cancellationToken.ThrowIfCancellationRequested();

        // Save client if new and at least one contract is creatable
        if (clientWasCreated && contractsToCreate.Count > 0)
        {
            await _clientRepository.SaveAsync(client);
        }

        // Save contracts
        foreach (var contract in contractsToCreate)
        {
            await _contractRepository.SaveAsync(contract);
        }

        // Determine final outcome
        var outcome = DetermineOutcome(
            contractsCreatedResult,
            contractsAlreadyExistingResult,
            contractsSkippedResult,
            contractsUnresolvedResult,
            globalErrors: new List<ImportAnalysisIssue>());

        return new BrioContractApplicationResult(
            client.Id,
            clientWasCreated,
            contractsCreatedResult,
            contractsAlreadyExistingResult,
            contractsSkippedResult,
            contractsUnresolvedResult,
            globalErrors: new List<ImportAnalysisIssue>(),
            globalWarnings,
            outcome);
    }

    private ApplicationOutcome DetermineOutcome(
        List<ContractCreated> created,
        List<ContractAlreadyExisting> alreadyExisting,
        List<ContractSkipped> skipped,
        List<ContractUnresolved> unresolved,
        List<ImportAnalysisIssue> globalErrors)
    {
        if (globalErrors.Count > 0)
            return ApplicationOutcome.Failed;

        var totalApplied = created.Count + alreadyExisting.Count;
        var totalProblematic = skipped.Count + unresolved.Count;

        if (totalApplied > 0 && totalProblematic == 0)
            return ApplicationOutcome.Success;

        if (totalApplied > 0 && totalProblematic > 0)
            return ApplicationOutcome.PartialSuccess;

        return ApplicationOutcome.Failed;
    }

    private BrioContractApplicationResult FailedResult(Guid? clientId, string errorCode, string errorMessage)
    {
        var globalError = new ImportAnalysisIssue(
            errorCode,
            ImportIssueSeverity.BlockingError,
            errorMessage,
            lineNumber: null,
            fieldName: null);

        return new BrioContractApplicationResult(
            clientId,
            clientWasCreated: false,
            contractsCreated: new List<ContractCreated>(),
            contractsAlreadyExisting: new List<ContractAlreadyExisting>(),
            contractsSkipped: new List<ContractSkipped>(),
            contractsUnresolved: new List<ContractUnresolved>(),
            globalErrors: new List<ImportAnalysisIssue> { globalError },
            globalWarnings: new List<ImportAnalysisIssue>(),
            ApplicationOutcome.Failed);
    }
}
