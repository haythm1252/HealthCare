using HealthCare.Application.Features.DoctorSlots.Contracts;
using HealthCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Doctors.Contracts;

public record DoctorBookingDetailsResponse(
    Guid Id,
    string Name,
    Guid SpecialtyId,
    string SpecialtyName,
    string Title,
    string Bio,
    string City,
    string Address,
    string? AddressUrl,
    string PhoneNumber,
    Gender Gender,
    decimal Rating,
    int RatingsCount,
    decimal ClinicFee,
    decimal HomeFee,
    decimal OnlineFee,
    bool AllowHomeVisit,
    bool AllowOnlineConsultation,
    string ProfilePictureUrl,
    IEnumerable<DailySlotsDto> Slots
);