using System.Collections.Generic;

namespace Praxis360_v1.Application.Models;

public sealed record BrioFileReadResult
{
    public IReadOnlyList<BrioRawLine> Lines { get; }
    public IReadOnlyList<string> StructuralErrors { get; }
    public int ExpectedColumnCount { get; }

    public BrioFileReadResult(
        IReadOnlyList<BrioRawLine> lines,
        IReadOnlyList<string> structuralErrors,
        int expectedColumnCount)
    {
        Lines = lines ?? throw new System.ArgumentNullException(nameof(lines));
        StructuralErrors = structuralErrors ?? throw new System.ArgumentNullException(nameof(structuralErrors));
        ExpectedColumnCount = expectedColumnCount;
    }

    public bool HasStructuralErrors => StructuralErrors.Count > 0;
}
