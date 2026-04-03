using HealthCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Appointments.Contracts;

public record UpdateAppointmentStatusRequest(
    string AppointmentType,
    string Status
);
