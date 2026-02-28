namespace HealthCare.Application.Common.Consts;

public static class RegexPatterns
{
    public const string Password = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
    public const string PhoneNumber = @"^\+?2?01[0125][0-9]{8}$";
}
