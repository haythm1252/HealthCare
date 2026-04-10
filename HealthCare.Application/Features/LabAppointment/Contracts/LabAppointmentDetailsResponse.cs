using HealthCare.Application.Features.Reviews.Contracts;
using HealthCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.LabAppointment.Contracts;

public record LabAppointmentDetailsResponse(
    Guid Id,
    Guid LabId,
    Guid PatientId,
    string LabName,
    AppointmentType AppointmentType,
    AppointmentStatus Status,
    DateOnly Date,
    TimeOnly StartTime,
    decimal TotalFee,
    string? Notes,
    string? Address,
    IEnumerable<TestResultDto> TestResults,
    ReviewResponse? Review
);
