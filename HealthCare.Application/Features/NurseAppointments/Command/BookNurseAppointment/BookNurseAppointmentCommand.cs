using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.NurseAppointments.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace HealthCare.Application.Features.NurseAppointments.Command.BookNurseAppointment;

public record BookNurseAppointmentCommand(
    string UserId,
    Guid NurseId,
    Guid ShiftId,
    string? Notes,
    string Address,
    string ServiceType,
    int? Hours
) : IRequest<Result<BookNurseAppointmentResponse>>;
