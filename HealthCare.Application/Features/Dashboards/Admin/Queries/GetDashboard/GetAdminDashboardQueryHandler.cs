using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.Dashboards.Admin.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.Application.Features.Dashboards.Admin.Queries.GetDashboard;

public class GetAdminDashboardQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAdminDashboardQuery, Result<AdminDashboardResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<AdminDashboardResponse>> Handle(GetAdminDashboardQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var thirtyDaysAgoDateTime = now.AddDays(-30);
        var thirtyDaysAgoDateOnly = DateOnly.FromDateTime(thirtyDaysAgoDateTime);


        // i prefer to do all in one query to only go to database one time 

        var stats = await _unitOfWork.Users.AsQueryable()
            .AsNoTracking()
            .Select(_ => new
            {
                TotalUsers = _unitOfWork.Users.AsQueryable().Count(u => !u.IsDisabled),
                NewUsers = _unitOfWork.Users.AsQueryable().Count(u => !u.IsDisabled && u.CreatedAt >= thirtyDaysAgoDateTime),

                Patients = _unitOfWork.Patients.AsQueryable().Count(p => !p.User.IsDisabled),
                Doctors = _unitOfWork.Doctors.AsQueryable().Count(d => !d.User.IsDisabled),
                Nurses = _unitOfWork.Nurses.AsQueryable().Count(n => !n.User.IsDisabled),
                Labs = _unitOfWork.Labs.AsQueryable().Count(l => !l.User.IsDisabled),

                OnlineRevenue = _unitOfWork.DoctorAppointments.AsQueryable()
                    .Where(a => (a.Status == AppointmentStatus.Confirmed || a.Status == AppointmentStatus.Completed)
                                && a.PaymentStatus == PaymentStatus.Paid && a.AppointmentType == AppointmentType.Online
                                && a.DoctorSlot.Date >= thirtyDaysAgoDateOnly)
                    .Sum(a => (decimal?)a.Fee ?? 0),

                DoctorRevenue = _unitOfWork.DoctorAppointments.AsQueryable()
                    .Where(a => a.Status == AppointmentStatus.Completed && a.DoctorSlot.Date >= thirtyDaysAgoDateOnly)
                    .Sum(a => (decimal?)a.Fee ?? 0),

                NurseRevenue = _unitOfWork.NurseAppointments.AsQueryable()
                    .Where(a => a.Status == AppointmentStatus.Completed && a.NurseShift.Date >= thirtyDaysAgoDateOnly)
                    .Sum(a => (decimal?)a.TotalFee ?? 0),

                LabRevenue = _unitOfWork.LabAppointments.AsQueryable()
                    .Where(a => a.Status == AppointmentStatus.Completed && a.Date >= thirtyDaysAgoDateOnly)
                    .Sum(a => (decimal?)a.TotalFee ?? 0)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (stats is null)
            return Result.Failure<AdminDashboardResponse>(UserErrors.NotFound);

        var specialties = await _unitOfWork.DoctorAppointments.AsQueryable()
            .AsNoTracking()
            .Where(a => (a.Status == AppointmentStatus.Confirmed || a.Status == AppointmentStatus.Completed)
                        && a.DoctorSlot.Date >= thirtyDaysAgoDateOnly)
            .GroupBy(a => a.Doctor.Specialty.Name)
            .ToListAsync(cancellationToken);

        var topSpecialties = specialties.Select(g => new SpecialtyStatDto(g.Key, g.Count()))
            .OrderByDescending(x => x.AppointmentsCount)
            .Take(5)
            .ToList();

        var response = new AdminDashboardResponse(
            stats.TotalUsers,
            stats.NewUsers,
            stats.Patients,
            stats.Doctors,
            stats.Nurses,
            stats.Labs,
            stats.OnlineRevenue,
            stats.DoctorRevenue,
            stats.NurseRevenue,
            stats.LabRevenue,
            topSpecialties
        );

        return Result.Success(response);





        ////users counts
        //var totalUsers = await _unitOfWork.Users.AsQueryable()
        //    .CountAsync(u => !u.IsDisabled, cancellationToken);

        //var newUsersLast30Days = await _unitOfWork.Users
        //    .CountAsync(u => !u.IsDisabled && u.CreatedAt.Date >= thirtyDaysAgoDateTime, cancellationToken);


        ////users count by role
        //var patientsCount = await _unitOfWork.Patients.CountAsync(p => !p.User.IsDisabled, cancellationToken);

        //var doctorsCount = await _unitOfWork.Doctors.CountAsync(d => !d.User.IsDisabled, cancellationToken);

        //var nursesCount = await _unitOfWork.Nurses.CountAsync(n => !n.User.IsDisabled, cancellationToken);

        //var labsCount = await _unitOfWork.Labs.CountAsync(l => !l.User.IsDisabled, cancellationToken);



        //// online revenue
        //var totalOnlineRevenue = await _unitOfWork.DoctorAppointments.AsQueryable()
        //    .Where(a => (a.Status == AppointmentStatus.Confirmed || a.Status == AppointmentStatus.Completed)
        //            && a.PaymentStatus == PaymentStatus.Paid &&
        //            a.AppointmentType == AppointmentType.Online && a.DoctorSlot.Date >= thirtyDaysAgoDateOnly)
        //    .SumAsync(a => a.Fee , cancellationToken);



        //// doctor nurse lab revenue
        //var totalDoctorRevenue = await _unitOfWork.DoctorAppointments.AsQueryable()
        //    .Where(a => a.Status == AppointmentStatus.Completed &&
        //                a.DoctorSlot.Date >= thirtyDaysAgoDateOnly)
        //    .SumAsync(a => a.Fee, cancellationToken);

        //var totalNurseRevenue = await _unitOfWork.NurseAppointments.AsQueryable()
        //    .Where(a => a.Status == AppointmentStatus.Completed &&
        //                a.NurseShift.Date >= thirtyDaysAgoDateOnly)
        //    .SumAsync(a => a.TotalFee, cancellationToken);

        //var totalLabRevenue = await _unitOfWork.LabAppointments.AsQueryable()
        //    .Where(a => a.Status == AppointmentStatus.Completed &&
        //                a.Date >= thirtyDaysAgoDateOnly)
        //    .SumAsync(a => a.TotalFee, cancellationToken);



        //// Get top 5 specialties by appointments count
        //var top5Specialties = await _unitOfWork.DoctorAppointments.AsQueryable()
        //    .Where(a => (a.Status == AppointmentStatus.Confirmed || a.Status == AppointmentStatus.Completed) &&
        //                a.DoctorSlot.Date >= thirtyDaysAgoDateOnly)
        //    .GroupBy(a => a.Doctor.Specialty.Name)
        //    .Select(g => new SpecialtyStatDto(g.Key, g.Count()))
        //    .OrderByDescending(x => x.AppointmentsCount)
        //    .Take(5)
        //    .ToListAsync(cancellationToken);

        //var response = new AdminDashboardResponse(
        //    totalUsers,
        //    newUsersLast30Days,
        //    patientsCount,
        //    doctorsCount,
        //    nursesCount,
        //    labsCount,
        //    totalOnlineRevenue,
        //    totalDoctorRevenue,
        //    totalNurseRevenue,
        //    totalLabRevenue,
        //    top5Specialties
        //);

        //return Result.Success(response);
    }
}
