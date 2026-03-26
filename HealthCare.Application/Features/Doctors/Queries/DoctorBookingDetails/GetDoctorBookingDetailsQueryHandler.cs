using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.Doctors.Contracts;
using HealthCare.Application.Features.DoctorSlots.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Domain.Enums;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Doctors.Queries.DoctorBookingDetails;

public class GetDoctorBookingDetailsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetDoctorBookingDetailsQuery, Result<DoctorBookingDetailsResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<DoctorBookingDetailsResponse>> Handle(GetDoctorBookingDetailsQuery request, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var doctorData = await _unitOfWork.Doctors.AsQueryable()
            .AsNoTracking()
            .AsSplitQuery()
            .Where(d => d.Id == request.Id && !d.User.IsDisabled)
            .Select(d => new
            {
                d.Id,
                d.User.Name,
                SpecialtyId = d.Specialty.Id,
                SpecialtyName = d.Specialty.Name,
                d.Title,
                d.Bio,
                d.User.City,
                d.User.Address,
                d.User.AddressUrl,
                d.User.PhoneNumber,
                d.User.Gender,
                d.Rating,
                d.RatingsCount,
                d.ClinicFee,
                d.HomeFee,
                d.OnlineFee,
                d.AllowHomeVisit,
                d.AllowOnlineConsultation,
                d.ProfilePictureUrl,

                DoctorSlots = d.DoctorSlots
                    .Where(s => s.Date >= today)
                    .Select(s => new { s.Id, s.Date, s.StartTime, s.EndTime, s.IsBooked })
                    .ToList()
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (doctorData is null)
            return Result.Failure<DoctorBookingDetailsResponse>(DoctorErrors.NotFound);

        var dailySlots = doctorData.DoctorSlots
            .GroupBy(s => s.Date)
            .OrderBy(g => g.Key)
            .Select(g => new DailySlotsDto(
                g.Key,
                g.Key.DayOfWeek.ToString(),
                g.OrderBy(s => s.StartTime)
                 .Select(s => new SlotDto(s.Id, s.StartTime, s.EndTime, s.IsBooked))
                 .ToList()
            )).ToList();

        //var response = doctorData.Adapt<DoctorBookingDetailsResponse>() with { Slots = dailySlots };

        var response = new DoctorBookingDetailsResponse(
            doctorData.Id,
            doctorData.Name,
            doctorData.SpecialtyId,
            doctorData.SpecialtyName,
            doctorData.Title,
            doctorData.Bio,
            doctorData.City,
            doctorData.Address,
            doctorData.AddressUrl,
            doctorData.PhoneNumber!,
            (Gender) doctorData.Gender!,
            doctorData.Rating,
            doctorData.RatingsCount,
            doctorData.ClinicFee,
            doctorData.HomeFee,
            doctorData.OnlineFee,
            doctorData.AllowHomeVisit,
            doctorData.AllowOnlineConsultation,
            doctorData.ProfilePictureUrl,
            dailySlots
        );

        return Result.Success(response);
    }
}
