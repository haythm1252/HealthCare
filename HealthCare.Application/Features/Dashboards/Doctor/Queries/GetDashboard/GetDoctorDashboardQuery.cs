using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Dashboards.Doctor.Contracts;
using MediatR;

namespace HealthCare.Application.Features.Dashboards.Doctor.Queries.GetDashboard;

public record GetDoctorDashboardQuery(
    string UserId
) : IRequest<Result<DoctorDashboardResponse>>;
