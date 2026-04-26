using HealthCare.Domain.Enums;

namespace HealthCare.Application.Features.Dashboards.Doctor.Contracts;

public record DoctorDashboardResponse(
    decimal Rate,
    int RateNumber,
    int CompletedAppointmentsLast30Days,
    decimal RevenueLast30Days,
    decimal OnlineRevenueLast30Days,
    int OnlineAppointmentsCount,
    int HomeAppointmentsCount,
    int OnsiteAppointmentsCount,
    IEnumerable<DoctorTodayAppointmentDto> TodayAppointments
);

public record DoctorTodayAppointmentDto(
    string PatientName,
    TimeOnly StartTime,
    TimeOnly EndTime,
    AppointmentStatus Status,
    AppointmentType AppointmentType
);


