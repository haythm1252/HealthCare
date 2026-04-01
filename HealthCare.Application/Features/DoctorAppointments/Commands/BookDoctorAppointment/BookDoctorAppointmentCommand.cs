using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.DoctorAppointments.Contracts;
using HealthCare.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.DoctorAppointments.Commands.BookDoctorAppointment;

public record BookDoctorAppointmentCommand(
    string UserId,
    Guid DoctorId,
    Guid DoctorSlotId,
    string AppointmentType,
    string? Notes,
    string? Address
) : IRequest<Result<BookDoctorAppointmentResponse>>;
