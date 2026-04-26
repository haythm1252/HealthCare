using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.Dashboards.Lab.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.Application.Features.Dashboards.Lab.Queries.GetDashboard;

public class GetLabDashboardQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetLabDashboardQuery, Result<LabDashboardResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<LabDashboardResponse>> Handle(GetLabDashboardQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var thirtyDaysAgo = DateOnly.FromDateTime(now.AddDays(-30));
        var todayDateOnly = DateOnly.FromDateTime(now);

        var labDash = await _unitOfWork.Labs.AsQueryable()
            .AsNoTracking()
            .AsSplitQuery()
            .Where(l => l.UserId == request.UserId)
            .Select(l => new
            {
                l.Rating,
                l.RatingsCount,

                CompletedAppointmentsLast30Days = l.LabAppointments
                    .Count(a => a.Status == AppointmentStatus.Completed && a.Date == thirtyDaysAgo),

                RevenueLast30Days = l.LabAppointments
                    .Where(a => a.Status == AppointmentStatus.Completed && a.Date >= thirtyDaysAgo)
                    .Sum(a => (decimal?)a.TotalFee ?? 0),

                AppointmentsByType = l.LabAppointments
                    .Where(a => (a.Status == AppointmentStatus.Confirmed || a.Status == AppointmentStatus.Completed) && a.Date >= thirtyDaysAgo)
                    .GroupBy(a => a.AppointmentType)
                    .Select(g => new { Type = g.Key, Count = g.Count() })
                    .ToList(),


                TodayAppointments = l.LabAppointments
                    .Where(a => (a.Status != AppointmentStatus.Declined && a.Status != AppointmentStatus.Pending) && a.Date == todayDateOnly)
                    .OrderBy(a => a.StartTime)
                    .Select(a => new LabTodayAppointmentDto(
                        a.Patient.User.Name,
                        a.StartTime,
                        a.Status,
                        a.AppointmentType,
                        a.TestResults.Select(tr => tr.Test.Name).ToList() 
                    )).ToList()

            }).SingleOrDefaultAsync(cancellationToken);

        if (labDash is null)
            return Result.Failure<LabDashboardResponse>(LabErrors.NotFound);

        // i use select many here because the test results are lists inside each appointment
        var bookedTests = await _unitOfWork.LabAppointments.AsQueryable()
            .Where(a => a.Lab.UserId == request.UserId && (a.Status == AppointmentStatus.Confirmed || a.Status == AppointmentStatus.Completed ||
                        a.Status == AppointmentStatus.ResultsDone) && a.Date >= thirtyDaysAgo)
            .SelectMany(a => a.TestResults)
            .GroupBy(tr => tr.Test.Name)
            .ToListAsync(cancellationToken);

        var mostBookedTests = bookedTests.Select(g => new MostBookedTestDto(g.Key, g.Count()))
            .OrderByDescending(x => x.BookingCount)
            .Take(5)
            .ToList();


        var homeCount = labDash.AppointmentsByType.FirstOrDefault(x => x.Type == AppointmentType.HomeVisit)?.Count ?? 0;
        var onsiteCount = labDash.AppointmentsByType.FirstOrDefault(x => x.Type == AppointmentType.OnSiteVisit)?.Count ?? 0;



        var response = new LabDashboardResponse(
            labDash.Rating,
            labDash.RatingsCount,
            labDash.CompletedAppointmentsLast30Days,
            labDash.RevenueLast30Days,
            homeCount,
            onsiteCount,
            mostBookedTests,
            labDash.TodayAppointments
        );

        return Result.Success(response);
    }
}
