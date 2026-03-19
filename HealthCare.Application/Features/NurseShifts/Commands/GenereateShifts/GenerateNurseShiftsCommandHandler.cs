using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.DoctorSlots.Contracts;
using HealthCare.Application.Features.NurseShifts.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Domain.Entities;
using HealthCare.Domain.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.NurseShifts.Commands.GenereateShifts;

public class GenerateNurseShiftsCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<GenerateNurseShiftsCommand, Result<IEnumerable<DailyShiftsDto>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<IEnumerable<DailyShiftsDto>>> Handle(GenerateNurseShiftsCommand request, CancellationToken cancellationToken)
    {
        var nurseId = await _unitOfWork.Nurses.AsQueryable()
            .Where(n => n.UserId == request.UserId)
            .Select(n => n.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (nurseId == Guid.Empty)
            return Result.Failure<IEnumerable<DailyShiftsDto>>(UserErrors.NotFound);

        var effectiveEndDate = request.EndDate ?? request.StartDate;

        // Delete existing unbooked shifts in the time in the during the time in the reqeust
        await _unitOfWork.NurseShifts.AsQueryable()
            .Where(ns => ns.NurseId == nurseId
                      && ns.Date >= request.StartDate
                      && ns.Date <= effectiveEndDate
                      && !ns.IsBooked
                      && ns.StartTime < request.EndTime
                      && ns.EndTime > request.StartTime)
            .ExecuteDeleteAsync(cancellationToken);

        // get the booked shifts to not create slot in the same time
        var bookedSlots = await _unitOfWork.NurseShifts.AsQueryable()
            .Where(ns => ns.NurseId == nurseId
                      && ns.Date >= request.StartDate
                      && ns.Date <= effectiveEndDate
                      && ns.IsBooked
                      && ns.StartTime < request.EndTime
                      && ns.EndTime > request.StartTime)
            .ToListAsync(cancellationToken);

        var shiftsToGenerate = new List<NurseShift>();

        // Generate shifts day by day
        for (var date = request.StartDate; date <= effectiveEndDate; date = date.AddDays(1))
        {
            // see if there is a booked slot in this time
            bool hasConflict = bookedSlots.Any(b => b.Date == date && request.StartTime < b.EndTime && request.EndTime > b.StartTime);

            if (!hasConflict)
                shiftsToGenerate.Add(new NurseShift 
                { 
                    NurseId = nurseId,
                    Date = date,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    IsBooked = false
                });

        }

        IEnumerable<NurseShift> savedShifts = [];

        if (shiftsToGenerate.Count != 0)
        {
            savedShifts = await _unitOfWork.NurseShifts.AddRangeAsync(shiftsToGenerate, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var response = savedShifts
            .GroupBy(s => s.Date)
            .OrderBy(g => g.Key)
            .Select(g => new DailyShiftsDto(
                g.Key,
                g.Key.DayOfWeek.ToString(),
                g.Select(s => new ShiftDto(s.Id, s.StartTime, s.EndTime, s.IsBooked))
                 .ToList()
            ));

        return Result.Success(response);
    }
}
