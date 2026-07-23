using System;
using System.Threading;
using System.Threading.Tasks;
using Praxis360.Domain.Types;
using Praxis360_v1.Application.Models;

namespace Praxis360_v1.Application.Interfaces;

public interface IBrioContractApplicationService
{
    Task<BrioContractApplicationResult> ApplyToExistingClientAsync(
        BrioImportAnalysisResult analysisResult,
        string selectedClientNormalizedIdentity,
        Guid existingClientId,
        CancellationToken cancellationToken = default);

    Task<BrioContractApplicationResult> ApplyWithNewClientAsync(
        BrioImportAnalysisResult analysisResult,
        string selectedClientNormalizedIdentity,
        Language preferredLanguage,
        CancellationToken cancellationToken = default);
}
