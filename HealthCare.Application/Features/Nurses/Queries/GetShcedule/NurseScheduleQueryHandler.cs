using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.Doctors.Contracts;
using HealthCare.Application.Features.Nurses.Contracts;
using HealthCare.Application.Features.NurseShifts.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Nurses.Queries.GetShcedule;

public class NurseScheduleQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<NurseScheduleQuery, Result<NurseScheduleResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<NurseScheduleResponse>> Handle(NurseScheduleQuery request, CancellationToken cancellationToken)
    {
        var nurseSechdule = await _unitOfWork.Nurses.AsQueryable()
            .AsNoTracking()
            .AsSplitQuery()
            .Where(n => n.UserId == request.UserId)
            .Select(n => new 
            {
                n.HomeVisitFee,
                n.HourPrice,
                
                Shifts = n.NurseShifts
                    .Select(ns => new {ns.Id, ns.Date, ns.StartTime, ns.EndTime, ns.IsBooked})
                    .ToList()

            }).SingleOrDefaultAsync(cancellationToken);

        if (nurseSechdule is null)
            return Result.Failure<NurseScheduleResponse>(UserErrors.NotFound);

        var shifts = nurseSechdule.Shifts
            .GroupBy(s => s.Date)
            .OrderBy(g => g.Key)
            .Select(g => new DailyShiftsDto(
                g.Key,
                g.Key.DayOfWeek.ToString(),
                g.OrderBy(s => s.StartTime)
                 .Select(s => new ShiftDto(s.Id, s.StartTime, s.EndTime, s.IsBooked))
                 .ToList()
            )).ToList();

        var nurseSechduleResponse = new NurseScheduleResponse(
                nurseSechdule.HomeVisitFee,
                nurseSechdule.HourPrice,
                shifts
            );

        return Result.Success(nurseSechduleResponse);
    }
}
