using System.Text;

namespace Praxis360_v1.Tests.TestSupport;

/// <summary>
/// Builder for creating synthetic BRIO CSV rows with exactly 62 columns.
/// All data is entirely fictional and must never be derived from real BRIO exports.
/// </summary>
public sealed class BrioSyntheticRowBuilder
{
    private readonly string[] _cells = new string[62];

    public BrioSyntheticRowBuilder()
    {
        // Initialize all cells to empty strings
        for (int i = 0; i < 62; i++)
        {
            _cells[i] = string.Empty;
        }
    }

    public BrioSyntheticRowBuilder WithInsuredLastName(string value)
    {
        _cells[0] = value;
        return this;
    }

    public BrioSyntheticRowBuilder WithInsuredFirstName(string value)
    {
        _cells[1] = value;
        return this;
    }

    public BrioSyntheticRowBuilder WithInsuredProfession(string value)
    {
        _cells[4] = value;
        return this;
    }

    public BrioSyntheticRowBuilder WithInsuredEmail(string value)
    {
        _cells[5] = value;
        return this;
    }

    public BrioSyntheticRowBuilder WithInsuredPhone(string value)
    {
        _cells[6] = value;
        return this;
    }

    public BrioSyntheticRowBuilder WithPolicyNumberPrimary(string value)
    {
        _cells[7] = value;
        return this;
    }

    public BrioSyntheticRowBuilder WithStatusLabel(string value)
    {
        _cells[8] = value;
        return this;
    }

    public BrioSyntheticRowBuilder WithPolicyholderINAMI(string value)
    {
        _cells[22] = value;
        return this;
    }

    public BrioSyntheticRowBuilder WithBirthDate(string value)
    {
        _cells[24] = value;
        return this;
    }

    public BrioSyntheticRowBuilder WithPolicyNumberRepeated(string value)
    {
        _cells[30] = value;
        return this;
    }

    public BrioSyntheticRowBuilder WithPolicyNumberThird(string value)
    {
        _cells[43] = value;
        return this;
    }

    public BrioSyntheticRowBuilder WithProductCode(string value)
    {
        _cells[47] = value;
        return this;
    }

    public BrioSyntheticRowBuilder WithProductLabel(string value)
    {
        _cells[48] = value;
        return this;
    }

    public BrioSyntheticRowBuilder WithCell(int index, string value)
    {
        if (index < 0 || index >= 62)
            throw new ArgumentOutOfRangeException(nameof(index), "Cell index must be between 0 and 61");

        _cells[index] = value;
        return this;
    }

    public BrioSyntheticRowBuilder WithPolicyNumberAll(string value)
    {
        _cells[7] = value;
        _cells[30] = value;
        _cells[43] = value;
        return this;
    }

    public string BuildCsvLine()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < 62; i++)
        {
            if (i > 0)
                sb.Append(';');

            var cell = _cells[i];
            if (cell.Contains(';') || cell.Contains('"') || cell.Contains('\n') || cell.Contains('\r'))
            {
                sb.Append('"');
                sb.Append(cell.Replace("\"", "\"\""));
                sb.Append('"');
            }
            else
            {
                sb.Append(cell);
            }
        }
        return sb.ToString();
    }

    public static string BuildHeader()
    {
        var headers = new[]
        {
            "InsuredLastName", "InsuredFirstName", "InsuredAddress", "InsuredCountryPostalCity",
            "InsuredProfession", "InsuredEmail", "InsuredPhone", "PolicyNumberPrimary",
            "StatusLabel", "DomainLabel", "PolicyholderName", "PolicyholderTypeValue",
            "PolicyholderTypeLabel", "PolicyholderDetailedName", "PolicyholderFirstName", "PolicyholderCountryPostalCity",
            "PolicyholderAddress", "PolicyholderBox", "PolicyholderPhone", "PolicyholderEmail",
            "PolicyholderVATNumber", "PolicyholderBIC", "PolicyholderINAMINumber", "BirthPlace",
            "BirthDate", "Income", "ProfessionLabel", "IdentityCardNumber",
            "Fees", "PolicyTypeValue1", "PolicyNumberRepeated", "PolicyTypeLabel1",
            "PolicyTypeValue2", "DomainValue1", "DomainLabel1", "VersionLabel",
            "VersionValue", "StatusValue", "StatusLabel2", "DomainValue2",
            "DomainLabel2", "PolicyTypeValue3", "PolicyTypeLabel2", "PolicyNumberThird",
            "PolicyCheckDigit", "ReductionYesNo", "ReductionDate", "ProductCompanyCode",
            "ProductCompanyLabel", "Maturity", "PeriodicityValue", "PeriodicityLabel",
            "NextPremiumDate", "LastPremiumDate", "TotalPremiumLastTerm", "InsuredName",
            "Reserve", "ReserveGuaranteedRate", "Capital", "MinimumDeathCapital",
            "AdditionalDeathCapital", "AnnualAnnuity"
        };

        return string.Join(";", headers);
    }
}
