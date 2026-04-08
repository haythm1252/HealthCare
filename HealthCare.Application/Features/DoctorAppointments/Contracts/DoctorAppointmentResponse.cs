using HealthCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.DoctorAppointments.Contracts;

public record DoctorAppointmentResponse(
    Guid Id,
    string PatientName,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    AppointmentStatus Status,
    AppointmentType AppointmentType,
    string? Address = null,
    string? PatientPhoneNumber = null,
    string? Notes = null,   
    Gender? Gender = null
);