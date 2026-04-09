using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.LabAppointment.Contracts;
using MediatR;

namespace HealthCare.Application.Features.LabAppointment.Queries.GetLabAppointmentDetails;

public record GetLabAppointmentDetailsQuery(
    string UserId,
    string UserRole,
    Guid AppointmentId
) : IRequest<Result<LabAppointmentDetailsResponse>>;
