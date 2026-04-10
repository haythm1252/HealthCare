using HealthCare.Application.Features.Reviews.Contracts;
using HealthCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.NurseAppointments.Contracts;

public record NurseAppointmentDetailsResponse(
    Guid Id,
    Guid NurseId,
    Guid PatientId,
    string NurseName,
    NurseServiceType ServiceType,
    AppointmentStatus Status,
    DateOnly Date,
    TimeOnly ShiftStartTime,
    TimeOnly ShiftEndTime,
    TimeOnly AppointmentStartTime,
    decimal TotalFee,
    string? Notes,
    string Address,
    int? Hours,
    ReviewResponse? Review
);
