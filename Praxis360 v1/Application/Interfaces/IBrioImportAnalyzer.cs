using System.Threading;
using System.Threading.Tasks;
using Praxis360_v1.Application.Models;

namespace Praxis360_v1.Application.Interfaces;

public interface IBrioImportAnalyzer
{
    Task<BrioImportAnalysisResult> AnalyzeAsync(BrioFileReadResult readResult, CancellationToken cancellationToken = default);
}
