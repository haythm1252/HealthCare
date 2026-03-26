using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.NurseAppointments.Contracts;

public record BookNurseAppointmentRequest(
    Guid NurseId,
    Guid ShiftId,
    string? Notes,
    string Address,
    string ServiceType,
    int? Hours
);
