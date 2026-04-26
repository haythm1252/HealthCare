using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.Dashboards.Doctor.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Domain.Enums;
using HealthCare.Domain.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.Application.Features.Dashboards.Doctor.Queries.GetDashboard;

public class GetDoctorDashboardQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetDoctorDashboardQuery, Result<DoctorDashboardResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<DoctorDashboardResponse>> Handle(GetDoctorDashboardQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var thirtyDaysAgo = DateOnly.FromDateTime(now.AddDays(-30));
        var todayDateOnly = DateOnly.FromDateTime(now);

        var doctorDash = await _unitOfWork.Doctors.AsQueryable()
            .AsNoTracking()
            .AsSplitQuery()
            .Where(d => d.UserId == request.UserId)
            .Select(d => new
            {
                d.Rating,
                d.RatingsCount,

                CompletedAppointmentsLast30Days =
                    d.DoctorAppointments.Where(a => a.Status == AppointmentStatus.Completed && a.DoctorSlot.Date >= thirtyDaysAgo).Count(),

                RevenueLast30Days = d.DoctorAppointments.Where(a => a.Status == AppointmentStatus.Completed && a.DoctorSlot.Date >= thirtyDaysAgo)
                    .Sum(a => a.Fee),

                OnlineRevenueLast30Days = d.DoctorAppointments.Where(a => (a.Status == AppointmentStatus.Confirmed || a.Status == AppointmentStatus.Completed) 
                    && a.PaymentStatus == PaymentStatus.Paid &&
                    a.AppointmentType == AppointmentType.Online && a.DoctorSlot.Date >= thirtyDaysAgo).Sum(a => a.Fee),

                AppointmentsByType = d.DoctorAppointments
                    .Where(a => (a.Status == AppointmentStatus.Confirmed || a.Status == AppointmentStatus.Completed) && a.DoctorSlot.Date >= thirtyDaysAgo)
                    .GroupBy(a => a.AppointmentType)
                    .Select(g => new { Type = g.Key, Count = g.Count() })
                    .ToList(),

                TodayConfirmedAppointments = d.DoctorAppointments.Where(a => (a.Status != AppointmentStatus.Declined && a.Status != AppointmentStatus.Pending)
                    && a.DoctorSlot.Date == todayDateOnly)
                    .OrderBy(a => a.DoctorSlot.StartTime)
                    .Select(a => new DoctorTodayAppointmentDto(
                        a.Patient.User.Name,
                        a.DoctorSlot.StartTime,
                        a.DoctorSlot.EndTime,
                        a.Status,
                        a.AppointmentType
                    )).ToList()

            }).SingleOrDefaultAsync(cancellationToken);

        if (doctorDash is null)
            return Result.Failure<DoctorDashboardResponse>(DoctorErrors.NotFound);

        var onlineCount = doctorDash.AppointmentsByType.FirstOrDefault(x => x.Type == AppointmentType.Online)?.Count ?? 0;
        var homeCount = doctorDash.AppointmentsByType.FirstOrDefault(x => x.Type == AppointmentType.HomeVisit)?.Count ?? 0;
        var onsiteCount = doctorDash.AppointmentsByType.FirstOrDefault(x => x.Type == AppointmentType.OnSiteVisit)?.Count ?? 0;

        var response = new DoctorDashboardResponse(
            doctorDash.Rating,
            doctorDash.RatingsCount,
            doctorDash.CompletedAppointmentsLast30Days,
            doctorDash.RevenueLast30Days,
            doctorDash.OnlineRevenueLast30Days,
            onlineCount,
            homeCount,
            onsiteCount,
            doctorDash.TodayConfirmedAppointments
        );

        return Result.Success(response);
    }
}
