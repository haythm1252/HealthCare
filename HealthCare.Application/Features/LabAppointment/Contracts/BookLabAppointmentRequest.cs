using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.LabAppointment.Contracts;

public record BookLabAppointmentRequest(
    Guid LabId,
    DateOnly Date,
    string AppointmentType,
    string? Notes,
    string? Address,
    IEnumerable<Guid> LabTestsIds
);
