using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Dashboards.Admin.Contracts;
using MediatR;

namespace HealthCare.Application.Features.Dashboards.Admin.Queries.GetDashboard;

public record GetAdminDashboardQuery(
    string UserId
) : IRequest<Result<AdminDashboardResponse>>;
