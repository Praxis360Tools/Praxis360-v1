using System.Collections.Generic;

namespace Praxis360_v1.Application.Models;

public sealed record BrioRawLine
{
    public int LineNumber { get; }
    public IReadOnlyList<string> Cells { get; }

    public BrioRawLine(int lineNumber, IReadOnlyList<string> cells)
    {
        LineNumber = lineNumber;
        Cells = cells ?? throw new System.ArgumentNullException(nameof(cells));
    }
}
