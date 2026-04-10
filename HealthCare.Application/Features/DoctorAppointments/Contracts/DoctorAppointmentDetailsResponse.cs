using HealthCare.Application.Features.Reviews.Contracts;
using HealthCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.DoctorAppointments.Contracts;

public record DoctorAppointmentDetailsResponse(
    Guid Id,
    Guid DoctorId,
    Guid PatientId,
    string DoctorName,
    AppointmentType AppointmentType,
    AppointmentStatus Status,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    decimal Fee,
    string? Notes,
    string? Address,
    PaymentType PaymentType,
    string? Diagnosis,
    string? Prescriptions,
    IEnumerable<RequiredTestDto> RequiredTests,
    ReviewResponse? Review
);
