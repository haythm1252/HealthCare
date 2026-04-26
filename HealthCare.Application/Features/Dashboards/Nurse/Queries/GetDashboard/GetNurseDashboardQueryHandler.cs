using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.Dashboards.Nurse.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.Application.Features.Dashboards.Nurse.Queries.GetDashboard;

public class GetNurseDashboardQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetNurseDashboardQuery, Result<NurseDashboardResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<NurseDashboardResponse>> Handle(GetNurseDashboardQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var thirtyDaysAgo = DateOnly.FromDateTime(now.AddDays(-30));
        var todayDateOnly = DateOnly.FromDateTime(now);

        var nurseDash = await _unitOfWork.Nurses.AsQueryable()
            .AsNoTracking()
            .AsSplitQuery()
            .Where(n => n.UserId == request.UserId)
            .Select(n => new
            {
                n.Rating,
                n.RatingsCount,

                CompletedAppointmentsLast30Days = n.NurseAppointments
                    .Count(a => a.Status == AppointmentStatus.Completed && a.NurseShift.Date >= thirtyDaysAgo),

                RevenueLast30Days = n.NurseAppointments
                    .Where(a => a.Status == AppointmentStatus.Completed && a.NurseShift.Date >= thirtyDaysAgo)
                    .Sum(a => a.TotalFee),

                AppointmentsByType = n.NurseAppointments
                    .Where(a => (a.Status == AppointmentStatus.Confirmed || a.Status == AppointmentStatus.Completed) && a.NurseShift.Date >= thirtyDaysAgo)
                    .GroupBy(a => a.ServiceType)
                    .Select(g => new { ServiceType = g.Key, Count = g.Count() })
                    .ToList(),

                TodayAppointments = n.NurseAppointments
                    .Where(a => (a.Status != AppointmentStatus.Declined && a.Status != AppointmentStatus.Pending) && a.NurseShift.Date == todayDateOnly)
                    .OrderBy(a => a.StartTime)
                    .Select(a => new NurseTodayAppointmentDto(
                        a.Patient.User.Name,
                        a.StartTime, 
                        a.Status,
                        a.ServiceType,
                        a.ServiceType == NurseServiceType.HourlyStay ? a.Hours : null
                    )).ToList()

            }).SingleOrDefaultAsync(cancellationToken);

        if (nurseDash is null)
            return Result.Failure<NurseDashboardResponse>(NurseErrors.NotFound);

        var quickVisitCount = nurseDash.AppointmentsByType
            .FirstOrDefault(x => x.ServiceType == NurseServiceType.QuickVisit)?.Count ?? 0;
        var hourlyStayCount = nurseDash.AppointmentsByType
            .FirstOrDefault(x => x.ServiceType == NurseServiceType.HourlyStay)?.Count ?? 0;

        var response = new NurseDashboardResponse(
            nurseDash.Rating,
            nurseDash.RatingsCount,
            nurseDash.CompletedAppointmentsLast30Days,
            nurseDash.RevenueLast30Days,
            quickVisitCount,
            hourlyStayCount,
            nurseDash.TodayAppointments
        );

        return Result.Success(response);
    }
}
