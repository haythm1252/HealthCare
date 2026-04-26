using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Dashboards.Nurse.Contracts;
using MediatR;

namespace HealthCare.Application.Features.Dashboards.Nurse.Queries.GetDashboard;

public record GetNurseDashboardQuery(
    string UserId
) : IRequest<Result<NurseDashboardResponse>>;
