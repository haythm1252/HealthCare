using HealthCare.Application.Common.Result;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Appointments.Commands.CancelAppointment;

public record CancelAppointmentCommand(
    string UserId,
    Guid AppointmentId,
    string AppointmentType
) : IRequest<Result>;
