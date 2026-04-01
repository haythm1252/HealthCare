using HealthCare.Application.Features.LabTest.Contracts;
using HealthCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.DoctorAppointments.Contracts;

public record BookDoctorAppointmentResponse(
    Guid AppointmentId,
    string DoctorName,
    string DoctorPhoneNumber,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string? Address,
    AppointmentType AppointmentType,
    decimal TotalFee,
    AppointmentStatus Status,
    string? PaymentIframe
);
