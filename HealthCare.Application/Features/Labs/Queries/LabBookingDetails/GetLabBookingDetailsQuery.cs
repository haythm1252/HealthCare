using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Labs.Contracts;
using MediatR;

namespace HealthCare.Application.Features.Labs.Queries.LabBookingDetails;

public record GetLabBookingDetailsQuery(Guid Id) : IRequest<Result<LabBookingDetailsResponse>>;
