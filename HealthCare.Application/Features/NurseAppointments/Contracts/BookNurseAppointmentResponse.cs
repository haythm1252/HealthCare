using HealthCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace HealthCare.Application.Features.NurseAppointments.Contracts;

public record BookNurseAppointmentResponse(
    Guid AppointmentId,
    string NurseName,
    string NursePhoneNumber,
    DateOnly Date,
    TimeOnly Time,
    string Address,
    NurseServiceType ServiceType,
    int? Hours,
    decimal TotalFee,
    AppointmentStatus Status = AppointmentStatus.Pending
);
