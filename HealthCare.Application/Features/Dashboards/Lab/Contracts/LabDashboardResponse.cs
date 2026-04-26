using HealthCare.Domain.Enums;

namespace HealthCare.Application.Features.Dashboards.Lab.Contracts;

public record LabDashboardResponse(
    decimal Rate,
    int RateNumber,
    int CompletedAppointmentsLast30Days,
    decimal RevenueLast30Days,
    int HomeAppointmentsCount,
    int OnsiteAppointmentsCount,
    IEnumerable<MostBookedTestDto> MostBookedTests,
    IEnumerable<LabTodayAppointmentDto> TodayAppointments
);

public record LabTodayAppointmentDto(
    string PatientName,
    TimeOnly StartTime,
    AppointmentStatus Status,
    AppointmentType AppointmentType,
    IEnumerable<string> Tests
);

public record MostBookedTestDto(string TestName, int BookingCount);

