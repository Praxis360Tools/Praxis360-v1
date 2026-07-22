namespace Praxis360_v1.Application.Constants;

/// <summary>
/// Column positions for BRIO CSV file format (0-based index).
/// Based on BRIO export format with 62 columns.
/// </summary>
public static class BrioColumnPositions
{
    // Client identity information (Insured person)
    public const int InsuredLastName = 0;
    public const int InsuredFirstName = 1;
    public const int InsuredAddress = 2;
    public const int InsuredCountryPostalCity = 3;
    public const int InsuredProfession = 4;
    public const int InsuredEmail = 5;
    public const int InsuredPhone = 6;

    // Primary policy number
    public const int PolicyNumberPrimary = 7;

    // Status and domain
    public const int StatusLabel = 8;
    public const int DomainLabel = 9;

    // Policyholder information
    public const int PolicyholderName = 10;
    public const int PolicyholderTypeValue = 11;
    public const int PolicyholderTypeLabel = 12;

    // Policyholder details
    public const int PolicyholderDetailedName = 13;
    public const int PolicyholderFirstName = 14;
    public const int PolicyholderCountryPostalCity = 15;
    public const int PolicyholderAddress = 16;
    public const int PolicyholderBox = 17;
    public const int PolicyholderPhone = 18;
    public const int PolicyholderEmail = 19;
    public const int PolicyholderVATNumber = 20;
    public const int PolicyholderBIC = 21;
    public const int PolicyholderINAMINumber = 22;

    // Birth information
    public const int BirthPlace = 23;
    public const int BirthDate = 24;

    // Professional information
    public const int Income = 25;
    public const int ProfessionLabel = 26;
    public const int IdentityCardNumber = 27;

    // Fees
    public const int Fees = 28;

    // Policy type information (first occurrence)
    public const int PolicyTypeValue1 = 29;

    // Repeated policy number
    public const int PolicyNumberRepeated = 30;

    // Policy type information (continuation)
    public const int PolicyTypeLabel1 = 31;
    public const int PolicyTypeValue2 = 32;
    public const int DomainValue1 = 33;
    public const int DomainLabel1 = 34;
    public const int VersionLabel = 35;
    public const int VersionValue = 36;
    public const int StatusValue = 37;
    public const int StatusLabel2 = 38;
    public const int DomainValue2 = 39;
    public const int DomainLabel2 = 40;
    public const int PolicyTypeValue3 = 41;
    public const int PolicyTypeLabel2 = 42;

    // Third occurrence of policy number
    public const int PolicyNumberThird = 43;
    public const int PolicyCheckDigit = 44;

    // Reduction information
    public const int ReductionYesNo = 45;
    public const int ReductionDate = 46;

    // Product information
    public const int ProductCompanyCode = 47;
    public const int ProductCompanyLabel = 48;

    // Dates and periodicity
    public const int Maturity = 49;
    public const int PeriodicityValue = 50;
    public const int PeriodicityLabel = 51;
    public const int NextPremiumDate = 52;
    public const int LastPremiumDate = 53;

    // Financial amounts
    public const int TotalPremiumLastTerm = 54;
    public const int InsuredName = 55;
    public const int Reserve = 56;
    public const int ReserveGuaranteedRate = 57;
    public const int Capital = 58;
    public const int MinimumDeathCapital = 59;
    public const int AdditionalDeathCapital = 60;
    public const int AnnualAnnuity = 61;

    // Expected column count
    public const int ExpectedColumnCount = 62;
}
