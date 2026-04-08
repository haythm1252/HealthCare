using HealthCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.NurseAppointments.Contracts;

public record NurseAppointmentResponse(
    Guid Id,
    string PatientName,
    DateOnly Date,
    TimeOnly ShiftStartTime,
    TimeOnly ShiftEndTime,
    TimeOnly AppointmentStartTime,
    AppointmentStatus Status,
    NurseServiceType ServiceType,
    string Address,
    int? Hours,
    string? PatientPhoneNumber,
    string? Notes,
    Gender? Gender
);
