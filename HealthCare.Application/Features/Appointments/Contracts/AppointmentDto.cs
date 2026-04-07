using HealthCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Appointments.Contracts;

public record AppointmentDto(
    Guid Id,
    string ProviderName,
    string ProviderImage, 
    TargetType Type,
    DateOnly Date, 
    TimeOnly Time,
    AppointmentStatus Status, 
    decimal Price, 
    string ServiceType, 
    DateTime ScheduledAt,
    string? Specialty 
);
