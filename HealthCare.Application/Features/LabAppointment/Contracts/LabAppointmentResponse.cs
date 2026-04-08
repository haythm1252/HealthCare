using HealthCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.LabAppointment.Contracts;

public record LabAppointmentResponse(
    Guid Id,
    string PatientName,
    DateOnly Date,
    TimeOnly StartTime,
    AppointmentStatus Status,
    AppointmentType AppointmentType,
    int TestsCount,
    string? Address,
    string? PatientPhoneNumber,
    string? Notes,
    Gender? Gender,
    IEnumerable<string>? Tests
);
