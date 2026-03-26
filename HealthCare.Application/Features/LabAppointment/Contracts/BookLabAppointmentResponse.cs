using HealthCare.Application.Features.LabTest.Contracts;
using HealthCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.LabAppointment.Contracts;

public record BookLabAppointmentResponse(
    Guid AppointmentId,
    string LabName,
    string LabPhoneNumber,
    DateOnly Date,
    string Address,
    AppointmentType AppointmentType,
    decimal TotalFee,
    AppointmentStatus Status,
    IEnumerable<LabTestResponse> Tests
);
