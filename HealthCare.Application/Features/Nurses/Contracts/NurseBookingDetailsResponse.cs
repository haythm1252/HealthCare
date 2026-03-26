using HealthCare.Application.Features.DoctorSlots.Contracts;
using HealthCare.Application.Features.NurseShifts.Contracts;
using HealthCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Nurses.Contracts;

public record NurseBookingDetailsResponse(
    Guid Id,
    string Name,
    string Bio,
    string City,
    string PhoneNumber,
    Gender Gender,
    decimal Rating,
    int RatingsCount,
    decimal HourPrice,
    decimal HomeVisitFee,
    string ProfilePictureUrl,
    IEnumerable<DailyShiftsDto> Shifts
);
