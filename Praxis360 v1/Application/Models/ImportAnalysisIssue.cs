namespace Praxis360_v1.Application.Models;

public sealed record ImportAnalysisIssue
{
    public string Code { get; }
    public ImportIssueSeverity Severity { get; }
    public string Message { get; }
    public int? LineNumber { get; }
    public string? FieldName { get; }

    public ImportAnalysisIssue(
        string code,
        ImportIssueSeverity severity,
        string message,
        int? lineNumber = null,
        string? fieldName = null)
    {
        Code = code ?? throw new System.ArgumentNullException(nameof(code));
        Severity = severity;
        Message = message ?? throw new System.ArgumentNullException(nameof(message));
        LineNumber = lineNumber;
        FieldName = fieldName;
    }
}
