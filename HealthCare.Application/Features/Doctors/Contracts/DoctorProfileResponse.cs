using HealthCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Doctors.Contracts
{
    public record DoctorProfileResponse(
        string Id,
        string Name,
        string Title,
        Gender Gender,
        string Email,
        string PhoneNumber,
        string Address,
        string? AddressUrl,
        string City,
        string Bio,
        string ProfilePictureUrl
    );
}
