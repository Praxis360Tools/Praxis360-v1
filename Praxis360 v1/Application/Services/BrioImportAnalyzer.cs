using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Praxis360_v1.Application.Constants;
using Praxis360_v1.Application.Interfaces;
using Praxis360_v1.Application.Models;

namespace Praxis360_v1.Application.Services;

public sealed class BrioImportAnalyzer : IBrioImportAnalyzer
{
    private static readonly Regex ScientificNotationRegex = new Regex(@"^\s*[\+\-]?\d+\.?\d*[eE][\+\-]?\d+\s*$", RegexOptions.Compiled);
    private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public Task<BrioImportAnalysisResult> AnalyzeAsync(BrioFileReadResult readResult, CancellationToken cancellationToken = default)
    {
        if (readResult is null)
            throw new ArgumentNullException(nameof(readResult));

        var structuralErrors = ConvertStructuralErrors(readResult.StructuralErrors);

        var analyzedLines = new List<BrioAnalyzedLine>();
        var clientCandidates = new Dictionary<string, BrioClientCandidate>(StringComparer.OrdinalIgnoreCase);
        var contractCandidatesByKey = new Dictionary<string, BrioContractCandidate>(StringComparer.OrdinalIgnoreCase);
        var processedLineSignatures = new Dictionary<string, int>();

        foreach (var rawLine in readResult.Lines)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var lineSignature = ComputeLineSignature(rawLine.Cells);
            var isDuplicate = processedLineSignatures.ContainsKey(lineSignature);

            var analyzedLine = AnalyzeLine(rawLine, isDuplicate);
            analyzedLines.Add(analyzedLine);

            if (!isDuplicate)
            {
                processedLineSignatures[lineSignature] = rawLine.LineNumber;
            }

            if (analyzedLine.HasBlockingErrors)
                continue;

            if (!string.IsNullOrWhiteSpace(analyzedLine.ClientNormalizedIdentity))
            {
                ExtractOrUpdateClientCandidate(rawLine.Cells, analyzedLine.ClientNormalizedIdentity, clientCandidates);
            }

            if (!string.IsNullOrWhiteSpace(analyzedLine.NormalizedPolicyNumber) &&
                !string.IsNullOrWhiteSpace(analyzedLine.ClientNormalizedIdentity))
            {
                ExtractOrUpdateContractCandidate(
                    rawLine.Cells,
                    analyzedLine.NormalizedPolicyNumber,
                    analyzedLine.ClientNormalizedIdentity,
                    rawLine.LineNumber,
                    analyzedLine.RawProductCode,
                    analyzedLine.RawProductLabel,
                    analyzedLine.MappedContractType,
                    contractCandidatesByKey);
            }
        }

        var result = new BrioImportAnalysisResult(
            analyzedLines,
            clientCandidates,
            contractCandidatesByKey.Values.ToList(),
            structuralErrors);

        return Task.FromResult(result);
    }

    private List<ImportAnalysisIssue> ConvertStructuralErrors(IReadOnlyList<string> structuralErrors)
    {
        var issues = new List<ImportAnalysisIssue>();
        foreach (var error in structuralErrors)
        {
            issues.Add(new ImportAnalysisIssue(
                "BRIO_STRUCTURAL_ERROR",
                ImportIssueSeverity.BlockingError,
                error,
                null,
                null));
        }
        return issues;
    }

    private string ComputeLineSignature(IReadOnlyList<string> cells)
    {
        var parts = new List<string>(cells.Count);
        foreach (var cell in cells)
        {
            var normalized = cell?.Trim() ?? string.Empty;
            parts.Add($"{normalized.Length}:{normalized}");
        }
        return string.Join("|", parts);
    }

    private BrioAnalyzedLine AnalyzeLine(BrioRawLine rawLine, bool isDuplicate)
    {
        var cells = rawLine.Cells;

        var firstName = NormalizeText(GetCellValue(cells, BrioColumnPositions.InsuredFirstName));
        var lastName = NormalizeText(GetCellValue(cells, BrioColumnPositions.InsuredLastName));
        var birthDateStr = GetCellValue(cells, BrioColumnPositions.BirthDate);
        var email = GetCellValue(cells, BrioColumnPositions.InsuredEmail);
        var inamiNumber = GetCellValue(cells, BrioColumnPositions.PolicyholderINAMINumber);

        var productCode = GetCellValue(cells, BrioColumnPositions.ProductCompanyCode);
        var productLabel = GetCellValue(cells, BrioColumnPositions.ProductCompanyLabel);
        var mappedType = BrioProductCodeMapping.MapProductCode(productCode);

        var tempIssues = new List<ImportAnalysisIssue>();

        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
        {
            tempIssues.Add(new ImportAnalysisIssue(
                "BRIO_CLIENT_NOT_IDENTIFIABLE",
                ImportIssueSeverity.BlockingError,
                "Nom ou prénom du client manquant.",
                rawLine.LineNumber,
                "InsuredFirstName/InsuredLastName"));

            var analyzedLine = new BrioAnalyzedLine(
                rawLine.LineNumber,
                null,
                null,
                productCode,
                productLabel,
                mappedType);
            foreach (var issue in tempIssues)
                analyzedLine.Issues.Add(issue);
            return analyzedLine;
        }

        DateOnly? parsedBirthDate = null;
        if (!string.IsNullOrWhiteSpace(birthDateStr))
        {
            if (DateOnly.TryParse(birthDateStr, out var bd))
            {
                parsedBirthDate = bd;
            }
            else
            {
                tempIssues.Add(new ImportAnalysisIssue(
                    "BRIO_INVALID_BIRTH_DATE",
                    ImportIssueSeverity.Warning,
                    $"Date de naissance invalide : {birthDateStr}",
                    rawLine.LineNumber,
                    "BirthDate"));
            }
        }

        var normalizedEmail = NormalizeEmail(email);
        if (!string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(normalizedEmail))
        {
            tempIssues.Add(new ImportAnalysisIssue(
                "BRIO_INVALID_EMAIL",
                ImportIssueSeverity.Warning,
                $"Email invalide : {email}",
                rawLine.LineNumber,
                "InsuredEmail"));
        }

        var normalizedInami = NormalizeInami(inamiNumber);

        var clientIdentity = BuildClientIdentity(
            lastName,
            firstName,
            normalizedInami,
            parsedBirthDate,
            normalizedEmail,
            rawLine.LineNumber,
            tempIssues);

        if (string.IsNullOrWhiteSpace(clientIdentity))
        {
            var analyzedLine = new BrioAnalyzedLine(
                rawLine.LineNumber,
                null,
                null,
                productCode,
                productLabel,
                mappedType);
            foreach (var issue in tempIssues)
                analyzedLine.Issues.Add(issue);
            return analyzedLine;
        }

        var policyNum7 = GetCellValue(cells, BrioColumnPositions.PolicyNumberPrimary);
        var policyNum30 = GetCellValue(cells, BrioColumnPositions.PolicyNumberRepeated);
        var policyNum43 = GetCellValue(cells, BrioColumnPositions.PolicyNumberThird);

        var normalizedPolicyNumber = ValidatePolicyNumber(
            policyNum7,
            policyNum30,
            policyNum43,
            rawLine.LineNumber,
            tempIssues);

        if (isDuplicate)
        {
            tempIssues.Add(new ImportAnalysisIssue(
                "BRIO_EXACT_DUPLICATE",
                ImportIssueSeverity.Warning,
                "Ligne dupliquée détectée (toutes les cellules sont identiques).",
                rawLine.LineNumber,
                null));
        }

        if (!string.IsNullOrWhiteSpace(productCode) && !BrioProductCodeMapping.IsKnownProductCode(productCode))
        {
            tempIssues.Add(new ImportAnalysisIssue(
                "BRIO_PRODUCT_UNKNOWN",
                ImportIssueSeverity.Warning,
                $"Code produit inconnu : {productCode}",
                rawLine.LineNumber,
                "ProductCompanyCode"));
        }

        var finalLine = new BrioAnalyzedLine(
            rawLine.LineNumber,
            clientIdentity,
            normalizedPolicyNumber,
            productCode,
            productLabel,
            mappedType);
        foreach (var issue in tempIssues)
            finalLine.Issues.Add(issue);

        return finalLine;
    }

    private string? BuildClientIdentity(
        string lastName,
        string firstName,
        string? normalizedInami,
        DateOnly? birthDate,
        string? normalizedEmail,
        int lineNumber,
        List<ImportAnalysisIssue> tempIssues)
    {
        if (!string.IsNullOrWhiteSpace(normalizedInami))
        {
            return $"INAMI:{normalizedInami}";
        }

        if (birthDate.HasValue)
        {
            return $"{lastName.ToUpperInvariant()}|{firstName.ToUpperInvariant()}|{birthDate.Value:yyyy-MM-dd}";
        }

        if (!string.IsNullOrWhiteSpace(normalizedEmail))
        {
            return $"{lastName.ToUpperInvariant()}|{firstName.ToUpperInvariant()}|{normalizedEmail}";
        }

        tempIssues.Add(new ImportAnalysisIssue(
            "BRIO_CLIENT_NOT_IDENTIFIABLE",
            ImportIssueSeverity.BlockingError,
            "Client non identifiable : INAMI, date de naissance et email manquants ou invalides.",
            lineNumber,
            null));

        return null;
    }

    private string? ValidatePolicyNumber(
        string? policyNum7,
        string? policyNum30,
        string? policyNum43,
        int lineNumber,
        List<ImportAnalysisIssue> tempIssues)
    {
        var candidates = new List<string>();

        if (IsScientificNotation(policyNum7))
        {
            tempIssues.Add(new ImportAnalysisIssue(
                "BRIO_POLICY_NUMBER_SCIENTIFIC_NOTATION",
                ImportIssueSeverity.BlockingError,
                $"Numéro de police en notation scientifique (colonne 7) : {policyNum7}",
                lineNumber,
                "PolicyNumberPrimary"));
            return null;
        }

        if (IsScientificNotation(policyNum30))
        {
            tempIssues.Add(new ImportAnalysisIssue(
                "BRIO_POLICY_NUMBER_SCIENTIFIC_NOTATION",
                ImportIssueSeverity.BlockingError,
                $"Numéro de police en notation scientifique (colonne 30) : {policyNum30}",
                lineNumber,
                "PolicyNumberRepeated"));
            return null;
        }

        if (IsScientificNotation(policyNum43))
        {
            tempIssues.Add(new ImportAnalysisIssue(
                "BRIO_POLICY_NUMBER_SCIENTIFIC_NOTATION",
                ImportIssueSeverity.BlockingError,
                $"Numéro de police en notation scientifique (colonne 43) : {policyNum43}",
                lineNumber,
                "PolicyNumberThird"));
            return null;
        }

        if (!string.IsNullOrWhiteSpace(policyNum7)) candidates.Add(policyNum7.Trim());
        if (!string.IsNullOrWhiteSpace(policyNum30)) candidates.Add(policyNum30.Trim());
        if (!string.IsNullOrWhiteSpace(policyNum43)) candidates.Add(policyNum43.Trim());

        if (candidates.Count == 0)
        {
            tempIssues.Add(new ImportAnalysisIssue(
                "BRIO_POLICY_NUMBER_MISSING",
                ImportIssueSeverity.BlockingError,
                "Numéro de police absent dans les colonnes 7, 30 et 43.",
                lineNumber,
                "PolicyNumber"));
            return null;
        }

        var distinctValues = candidates.Distinct(StringComparer.OrdinalIgnoreCase).ToList();

        if (distinctValues.Count > 1)
        {
            tempIssues.Add(new ImportAnalysisIssue(
                "BRIO_POLICY_NUMBER_CONFLICT",
                ImportIssueSeverity.BlockingError,
                $"Numéros de police contradictoires : {string.Join(", ", distinctValues)}",
                lineNumber,
                "PolicyNumber"));
            return null;
        }

        if (candidates.Count == 1)
        {
            tempIssues.Add(new ImportAnalysisIssue(
                "BRIO_POLICY_NUMBER_SINGLE_OCCURRENCE",
                ImportIssueSeverity.Warning,
                "Une seule occurrence du numéro de police trouvée parmi les colonnes 7, 30 et 43.",
                lineNumber,
                "PolicyNumber"));
        }

        return distinctValues[0].ToUpperInvariant();
    }

    private bool IsScientificNotation(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return ScientificNotationRegex.IsMatch(value);
    }

    private string? NormalizeText(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        var normalized = text.Trim();
        normalized = Regex.Replace(normalized, @"\s+", " ");
        return normalized;
    }

    private string? NormalizeEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        var trimmed = email.Trim();
        if (!EmailRegex.IsMatch(trimmed))
            return null;

        return trimmed.ToLowerInvariant();
    }

    private string? NormalizeInami(string? inamiNumber)
    {
        if (string.IsNullOrWhiteSpace(inamiNumber))
            return null;

        var cleaned = Regex.Replace(inamiNumber, @"[\s\-\.\,\/]", "");

        if (cleaned.Length < 5)
            return null;

        return cleaned.ToUpperInvariant();
    }

    private void ExtractOrUpdateClientCandidate(
        IReadOnlyList<string> cells,
        string normalizedIdentity,
        Dictionary<string, BrioClientCandidate> clientCandidates)
    {
        if (clientCandidates.ContainsKey(normalizedIdentity))
            return;

        var firstName = NormalizeText(GetCellValue(cells, BrioColumnPositions.InsuredFirstName))!;
        var lastName = NormalizeText(GetCellValue(cells, BrioColumnPositions.InsuredLastName))!;

        var client = new BrioClientCandidate(normalizedIdentity, firstName, lastName);

        var birthDateStr = GetCellValue(cells, BrioColumnPositions.BirthDate);
        if (!string.IsNullOrWhiteSpace(birthDateStr) && DateOnly.TryParse(birthDateStr, out var birthDate))
        {
            client.SetDateOfBirth(birthDate);
        }

        var email = NormalizeEmail(GetCellValue(cells, BrioColumnPositions.InsuredEmail));
        var phone = GetCellValue(cells, BrioColumnPositions.InsuredPhone);
        client.SetContactInfo(email, phone);

        var profession = NormalizeText(GetCellValue(cells, BrioColumnPositions.InsuredProfession));
        var inamiNumber = NormalizeInami(GetCellValue(cells, BrioColumnPositions.PolicyholderINAMINumber));
        client.SetProfessionalInfo(profession, inamiNumber);

        clientCandidates[normalizedIdentity] = client;
    }

    private void ExtractOrUpdateContractCandidate(
        IReadOnlyList<string> cells,
        string normalizedPolicyNumber,
        string clientNormalizedIdentity,
        int lineNumber,
        string? rawProductCode,
        string? rawProductLabel,
        Praxis360.Domain.Types.ContractType? mappedType,
        Dictionary<string, BrioContractCandidate> contractCandidates)
    {
        var contractKey = $"{clientNormalizedIdentity}|{normalizedPolicyNumber}";

        if (contractCandidates.TryGetValue(contractKey, out var existing))
        {
            existing.AddSourceLine(lineNumber);
            return;
        }

        var insurerName = GetCellValue(cells, BrioColumnPositions.ProductCompanyLabel);
        var status = GetCellValue(cells, BrioColumnPositions.StatusLabel);

        var contract = new BrioContractCandidate(
            normalizedPolicyNumber,
            clientNormalizedIdentity,
            mappedType,
            rawProductCode,
            rawProductLabel,
            insurerName,
            status,
            lineNumber);

        contractCandidates[contractKey] = contract;
    }

    private static string? GetCellValue(IReadOnlyList<string> cells, int index)
    {
        if (index < 0 || index >= cells.Count)
            return null;

        var value = cells[index];
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
