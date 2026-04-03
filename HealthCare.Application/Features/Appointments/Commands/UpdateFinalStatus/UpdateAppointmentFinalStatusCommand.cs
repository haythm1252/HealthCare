using HealthCare.Application.Common.Result;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Appointments.Commands.UpdateFinalStatus;

public record UpdateAppointmentFinalStatusCommand(
    string UserId,
    string UserRole,
    Guid AppointmentId,
    string AppointmentType,
    string Status
) : IRequest<Result>;
