using System.Text.RegularExpressions;

namespace Praxis360_v1.Tests.TestSupport;

/// <summary>
/// Guards against accidental inclusion of real BRIO data in synthetic fixtures.
/// Validates that all CSV data is genuinely synthetic and contains no real customer information.
/// </summary>
public sealed class BrioSyntheticDataGuard
{
    private static readonly string[] ApprovedFictionalNames = new[]
    {
        "ALPHA", "BETA", "GAMMA", "DELTA", "EPSILON",
        "WARNING", "UNRESOLVED", "BLOCKING", "TEST", "SYNTHETIC",
        "EXAMPLE", "FICTIF", "FICTIVE", "DEMO", "SAMPLE"
    };

    private static readonly Regex EmailDomainRegex = new(@"@([a-zA-Z0-9.-]+)$", RegexOptions.Compiled);
    private static readonly string[] ApprovedEmailDomains = new[] { "example.test", "example.com", "test.local" };

    public static ValidationResult ValidateFixture(string fixturePath, string fixtureName)
    {
        var result = new ValidationResult { FixtureName = fixtureName, FixturePath = fixturePath };

        if (!File.Exists(fixturePath))
        {
            result.AddError($"Fixture file not found: {fixturePath}");
            return result;
        }

        // Check file size before reading content (50 KB max for synthetic fixtures)
        const long MaxFixtureBytes = 50_000;
        var fileInfo = new FileInfo(fixturePath);
        if (fileInfo.Length > MaxFixtureBytes)
        {
            result.AddError($"Fixture file is too large ({fileInfo.Length} bytes). Synthetic fixtures must not exceed {MaxFixtureBytes} bytes.");
            return result;
        }

        var lines = File.ReadAllLines(fixturePath);

        // Empty file is allowed (BrioSynthetic.Empty.csv)
        if (lines.Length == 0)
        {
            return result;
        }

        // Validate header if present
        if (lines.Length > 0 && !string.IsNullOrWhiteSpace(lines[0]))
        {
            ValidateHeader(lines[0], result);
        }

        // Validate data lines
        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i];
            if (string.IsNullOrWhiteSpace(line))
                continue;

            ValidateLine(line, i + 1, result, fixtureName);
        }

        return result;
    }

    private static void ValidateHeader(string headerLine, ValidationResult result)
    {
        var cells = headerLine.Split(';');

        // For invalid column count fixtures, we allow incorrect header
        if (cells.Length != 62 && !result.FixtureName.Contains("InvalidColumnCount", StringComparison.OrdinalIgnoreCase))
        {
            result.AddError($"Header must contain exactly 62 columns, found {cells.Length}");
        }
    }

    private static void ValidateLine(string line, int lineNumber, ValidationResult result, string fixtureName)
    {
        var cells = ParseCsvLine(line);

        // Check column count (allow incorrect count for InvalidColumnCount fixture)
        if (!fixtureName.Contains("InvalidColumnCount", StringComparison.OrdinalIgnoreCase) && cells.Count != 62)
        {
            result.AddWarning($"Line {lineNumber}: Expected 62 columns, found {cells.Count}");
        }

        if (cells.Count < 49) // Need at least enough cells to validate key fields
            return;

        // Validate names (columns 0, 1)
        ValidateName(cells[0], lineNumber, "InsuredLastName", result);
        ValidateName(cells[1], lineNumber, "InsuredFirstName", result);

        // Validate email (column 5)
        if (!string.IsNullOrWhiteSpace(cells[5]))
        {
            ValidateEmail(cells[5], lineNumber, result);
        }

        // Validate phone (column 6) - should not be a real-looking phone number
        if (!string.IsNullOrWhiteSpace(cells[6]))
        {
            ValidatePhone(cells[6], lineNumber, result);
        }

        // Validate policy numbers (columns 7, 30, 43) - must use SYN- prefix
        ValidatePolicyNumber(cells[7], lineNumber, "PolicyNumberPrimary", result);
        if (cells.Count > 30)
            ValidatePolicyNumber(cells[30], lineNumber, "PolicyNumberRepeated", result);
        if (cells.Count > 43)
            ValidatePolicyNumber(cells[43], lineNumber, "PolicyNumberThird", result);

        // Validate INAMI (column 22) - must be synthetic
        if (cells.Count > 22 && !string.IsNullOrWhiteSpace(cells[22]))
        {
            ValidateINAMI(cells[22], lineNumber, result);
        }

        // Validate product label (column 48) - should not contain real insurer names
        if (cells.Count > 48 && !string.IsNullOrWhiteSpace(cells[48]))
        {
            ValidateProductLabel(cells[48], lineNumber, result);
        }
    }

    private static void ValidateName(string name, int lineNumber, string fieldName, ValidationResult result)
    {
        if (string.IsNullOrWhiteSpace(name))
            return;

        var normalizedName = name.Trim().ToUpperInvariant();

        // Check if name contains at least one approved fictional keyword
        bool isApproved = ApprovedFictionalNames.Any(keyword => normalizedName.Contains(keyword));

        if (!isApproved)
        {
            result.AddError($"Line {lineNumber}, {fieldName}: '{name}' does not contain an approved fictional keyword. " +
                          $"Use names containing: {string.Join(", ", ApprovedFictionalNames)}");
        }
    }

    private static void ValidateEmail(string email, int lineNumber, ValidationResult result)
    {
        var match = EmailDomainRegex.Match(email);
        if (!match.Success)
        {
            result.AddWarning($"Line {lineNumber}: Email format could not be validated");
            return;
        }

        var domain = match.Groups[1].Value.ToLowerInvariant();
        if (!ApprovedEmailDomains.Contains(domain))
        {
            result.AddError($"Line {lineNumber}: Email domain '{domain}' is not approved. " +
                          $"Use only: {string.Join(", ", ApprovedEmailDomains)}");
        }
    }

    private static void ValidatePhone(string phone, int lineNumber, ValidationResult result)
    {
        // Remove common separators
        var digitsOnly = new string(phone.Where(char.IsDigit).ToArray());

        // Belgian phone numbers are typically 9-10 digits
        // Warn if it looks like a real phone number pattern
        if (digitsOnly.Length >= 9 && digitsOnly.Length <= 10)
        {
            result.AddWarning($"Line {lineNumber}: Phone number '{phone}' looks like it could be a real phone number. " +
                            "Use obviously synthetic patterns like +32-SYN-12345");
        }
    }

    private static void ValidatePolicyNumber(string policyNumber, int lineNumber, string fieldName, ValidationResult result)
    {
        if (string.IsNullOrWhiteSpace(policyNumber))
            return;

        var trimmed = policyNumber.Trim().ToUpperInvariant();

        // Policy numbers should be prefixed with SYN- or be scientific notation for testing
        if (!trimmed.StartsWith("SYN-") && !trimmed.Contains("E+") && !trimmed.Contains("E-"))
        {
            result.AddError($"Line {lineNumber}, {fieldName}: Policy number '{policyNumber}' must start with 'SYN-' prefix " +
                          "or be scientific notation for specific test cases");
        }
    }

    private static void ValidateINAMI(string inami, int lineNumber, ValidationResult result)
    {
        var trimmed = inami.Trim().ToUpperInvariant();

        // INAMI should be prefixed with SYN- or contain TEST/SYNTHETIC keywords
        if (!trimmed.StartsWith("SYN-") && !trimmed.Contains("TEST") && !trimmed.Contains("SYNTHETIC"))
        {
            result.AddError($"Line {lineNumber}: INAMI '{inami}' does not appear synthetic. " +
                          "Use 'SYN-' prefix or include TEST/SYNTHETIC keywords");
        }
    }

    private static void ValidateProductLabel(string label, int lineNumber, ValidationResult result)
    {
        var normalizedLabel = label.Trim().ToUpperInvariant();

        // Block known real Belgian insurance company names
        var suspiciousPatterns = new[] { "AG INSURANCE", "ETHIAS", "BELFIUS", "AXA", "BALOISE", "ALLIANZ", "KBC" };

        foreach (var pattern in suspiciousPatterns)
        {
            if (normalizedLabel.Contains(pattern))
            {
                result.AddError($"Line {lineNumber}: Product label '{label}' contains what appears to be a real insurer name. " +
                              "Use only fictional names like 'SYNTHETIC INSURER', 'TEST COMPANY', etc.");
            }
        }
    }

    private static List<string> ParseCsvLine(string line)
    {
        var cells = new List<string>();
        var currentCell = new System.Text.StringBuilder();
        var insideQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            var current = line[i];

            if (current == '"')
            {
                if (insideQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    currentCell.Append('"');
                    i++;
                }
                else
                {
                    insideQuotes = !insideQuotes;
                }
            }
            else if (current == ';' && !insideQuotes)
            {
                cells.Add(currentCell.ToString());
                currentCell.Clear();
            }
            else
            {
                currentCell.Append(current);
            }
        }

        cells.Add(currentCell.ToString());
        return cells;
    }

    public class ValidationResult
    {
        public string FixtureName { get; set; } = string.Empty;
        public string FixturePath { get; set; } = string.Empty;
        public List<string> Errors { get; } = new();
        public List<string> Warnings { get; } = new();

        public bool IsValid => Errors.Count == 0;

        public void AddError(string error) => Errors.Add(error);
        public void AddWarning(string warning) => Warnings.Add(warning);

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"Validation of {FixtureName}:");
            sb.AppendLine($"  Status: {(IsValid ? "VALID" : "INVALID")}");

            if (Errors.Count > 0)
            {
                sb.AppendLine($"  Errors ({Errors.Count}):");
                foreach (var error in Errors)
                    sb.AppendLine($"    - {error}");
            }

            if (Warnings.Count > 0)
            {
                sb.AppendLine($"  Warnings ({Warnings.Count}):");
                foreach (var warning in Warnings)
                    sb.AppendLine($"    - {warning}");
            }

            return sb.ToString();
        }
    }
}
