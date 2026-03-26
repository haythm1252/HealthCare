using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.LabAppointment.Contracts;
using MediatR;

namespace HealthCare.Application.Features.LabAppointment.Commands.BookLabAppointment;

public record BookLabAppointmentCommand(
    string UserId,
    Guid LabId,
    DateOnly Date,
    string AppointmentType,
    string? Notes,
    string? Address,
    IEnumerable<Guid> LabTestsIds
) : IRequest<Result<BookLabAppointmentResponse>>;
