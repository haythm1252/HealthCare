using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Dashboards.Lab.Contracts;
using MediatR;

namespace HealthCare.Application.Features.Dashboards.Lab.Queries.GetDashboard;

public record GetLabDashboardQuery(
    string UserId
) : IRequest<Result<LabDashboardResponse>>;
