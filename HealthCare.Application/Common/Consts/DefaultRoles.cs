namespace HealthCare.Application.Common.Consts;

public static class DefaultRoles
{
    public const string Admin = "Admin";
    public const string Patient = "Patient";
    public const string Doctor = "Doctor";
    public const string Nurse = "Nurse";
    public const string Lab = "Lab";

    public const string Model = "Model";

    public static readonly IReadOnlyList<string> AllRoles = [Admin, Patient, Doctor, Nurse, Lab];

    public static readonly IReadOnlyList<string> MedicalStaffRoles = [Doctor, Nurse, Lab];
}
