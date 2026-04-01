using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.DoctorAppointments.Contracts;

public record BookDoctorAppointmentRequest(
    Guid DoctorId,
    Guid DoctorSlotId,
    string AppointmentType,
    string? Notes,
    string? Address
);
