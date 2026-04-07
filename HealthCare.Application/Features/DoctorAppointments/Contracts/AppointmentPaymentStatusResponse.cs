using HealthCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.DoctorAppointments.Contracts;

public record AppointmentPaymentStatusResponse(
    Guid AppointmentId,
    string DoctorName,
    string DoctorPhoneNumber,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    AppointmentType AppointmentType,
    decimal TotalFee,
    AppointmentStatus Status,
    PaymentType PaymentType,
    PaymentStatus PaymentStatus
);
