using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Appointments.Contracts;

public record GetAppointmentsRequest(
    string? Search,
    string? Status,
    string? AppointmentType,
    int Page = 1,
    int PageSize = 20
);

