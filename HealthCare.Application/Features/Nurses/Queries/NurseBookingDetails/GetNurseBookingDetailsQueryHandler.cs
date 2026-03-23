using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.Nurses.Contracts;
using HealthCare.Application.Features.NurseShifts.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Domain.Enums;
using HealthCare.Domain.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Nurses.Queries.NurseBookingDetails;

public class GetNurseBookingDetailsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetNurseBookingDetailsQuery, Result<NurseBookingDetailsResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<NurseBookingDetailsResponse>> Handle(GetNurseBookingDetailsQuery request, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var nurseData = await _unitOfWork.Nurses.AsQueryable()
            .AsNoTracking()
            .AsSplitQuery()
            .Where(n => n.Id == request.Id && !n.User.IsDisabled)
            .Select(n => new
            {
                n.Id,
                n.User.Name,
                n.Bio,
                n.User.City,
                n.User.Gender,
                n.Rating,
                n.RatingsCount,
                n.HourPrice,
                n.HomeVisitFee,
                n.ProfilePictureUrl,

                NurseShifts = n.NurseShifts
                    .Where(s => s.Date >= today)
                    .Select(ns => new { ns.Id, ns.Date, ns.StartTime, ns.EndTime, ns.IsBooked })
                    .ToList()
            })
            .SingleOrDefaultAsync(cancellationToken);

            if (nurseData is null)
            return Result.Failure<NurseBookingDetailsResponse>(NurseErrors.NotFound);

        var shifts = nurseData.NurseShifts
            .GroupBy(s => s.Date)
            .OrderBy(g => g.Key)
            .Select(g => new DailyShiftsDto(
                g.Key,
                g.Key.DayOfWeek.ToString(),
                g.OrderBy(s => s.StartTime)
                 .Select(s => new ShiftDto(s.Id, s.StartTime, s.EndTime, s.IsBooked))
                 .ToList()
            )).ToList();

        var response = new NurseBookingDetailsResponse(
                nurseData.Id,
                nurseData.Name,
                nurseData.Bio,
                nurseData.City,
                (Gender) nurseData.Gender!,
                nurseData.Rating,
                nurseData.RatingsCount,
                nurseData.HourPrice,
                nurseData.HomeVisitFee,
                nurseData.ProfilePictureUrl,
                shifts
        );

        return Result.Success(response);
    }

}
