namespace HealthCare.Application.Common.Consts;

public static class DefaultRoles
{
    public const string Admin = "Admin";
    public const string Patient = "Patient";
    public const string Doctor = "Doctor";
    public const string Nurse = "Nurse";
    public const string Lab = "Lab";

    public static readonly IReadOnlyList<string> AllRoles = new List<string> { Admin, Patient, Doctor, Nurse, Lab };
}
