using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.Doctors.Contracts;
using HealthCare.Application.Features.DoctorSlots.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Doctors.Queries.GetSchedule;

public class DoctorScheduleQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<DoctorScheduleQuery, Result<DoctorScheduleResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<DoctorScheduleResponse>> Handle(DoctorScheduleQuery request, CancellationToken cancellationToken)
    {
        var doctorData = await _unitOfWork.Doctors
            .AsQueryable()
            .AsNoTracking()
            .AsSplitQuery()
            .Where(d => d.UserId == request.UserId)
            .Select(d => new
            {
                d.ClinicFee,
                d.HomeFee,
                d.OnlineFee,
                d.AllowHomeVisit,
                d.AllowOnlineConsultation,

                Slots = d.DoctorSlots
                    .Where(s => s.Date >= DateOnly.FromDateTime(DateTime.UtcNow))
                    .Select(s => new { s.Id, s.Date, s.StartTime, s.EndTime, s.IsBooked })
                    .ToList()    
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (doctorData is null) 
            return Result.Failure<DoctorScheduleResponse>(UserErrors.NotFound);
        
        var dailySlots = doctorData.Slots
            .GroupBy(s => s.Date)
            .OrderBy(g => g.Key)
            .Select(g => new DailySlotsDto(
                g.Key,
                g.Key.DayOfWeek.ToString(),
                g.OrderBy(s => s.StartTime)
                 .Select(s => new SlotDto(s.Id, s.StartTime, s.EndTime, s.IsBooked))
                 .ToList()
            )).ToList();

        var response = new DoctorScheduleResponse(
                doctorData.ClinicFee,
                doctorData.HomeFee,
                doctorData.OnlineFee,
                doctorData.AllowHomeVisit,
                doctorData.AllowOnlineConsultation,
                dailySlots
            );

        return Result.Success(response);
    }
}
