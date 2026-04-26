using HealthCare.Domain.Enums;

namespace HealthCare.Application.Features.Dashboards.Nurse.Contracts;

public record NurseDashboardResponse(
    decimal Rate,
    int RateNumber,
    int CompletedAppointmentsLast30Days,
    decimal RevenueLast30Days,
    int QuickVisitCount,
    int HourlyStayCount,
    IEnumerable<NurseTodayAppointmentDto> TodayAppointments
);

public record NurseTodayAppointmentDto(
    string PatientName,
    TimeOnly StartTime,
    AppointmentStatus Status,
    NurseServiceType ServiceType,
    int? Hours
);

