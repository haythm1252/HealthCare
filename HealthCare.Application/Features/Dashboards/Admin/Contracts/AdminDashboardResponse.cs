namespace HealthCare.Application.Features.Dashboards.Admin.Contracts;

public record AdminDashboardResponse(
    int TotalUsers,
    int NewUsersThisMonth,
    int PatientsCount,
    int DoctorsCount,
    int NursesCount,
    int LabsCount,
    decimal TotalOnlineRevenueLast30Days,
    decimal TotalDoctorRevenueLast30Days,
    decimal TotalNurseRevenueLast30Days,
    decimal TotalLabRevenueLast30Days,
    IEnumerable<SpecialtyStatDto> TopSpecialties
);

public record SpecialtyStatDto(string SpecialtyName, int AppointmentsCount);
