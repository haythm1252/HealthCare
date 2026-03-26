using HealthCare.Application.Features.DoctorSlots.Contracts;
using HealthCare.Application.Features.LabTest.Contracts;
using HealthCare.Domain.Entities;
using HealthCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Labs.Contracts;

public record LabBookingDetailsResponse(
    Guid Id,
    string Name,
    string Bio,
    string City,
    string Address,
    string? AddressUrl,
    string PhoneNumber,
    decimal Rating,
    int RatingsCount,
    decimal HomeVisitFee,
    string ProfilePictureUrl,
    TimeOnly OpeningTime,
    TimeOnly ClosingTime,
    WorkingDays WorkingDays,
    IEnumerable<LabTestResponse> Tests
);