using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.NurseAppointments.Contracts;
using MediatR;

namespace HealthCare.Application.Features.NurseAppointments.Queries.GetNurseAppointmentDetails;

public record GetNurseAppointmentDetailsQuery(
    string UserId,
    string UserRole,
    Guid AppointmentId
) : IRequest<Result<NurseAppointmentDetailsResponse>>;
