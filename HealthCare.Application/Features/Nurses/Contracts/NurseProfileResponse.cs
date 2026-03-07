using System;
using System.Collections.Generic;
using System.Text;
using HealthCare.Domain.Enums;

namespace HealthCare.Application.Features.Nurses.Contracts
{
    public record NurseProfileResponse(
    
    string Id,
    string Name,
    Gender Gender,
    string Email,
    string PhoneNumber,
    string Address,
    string? AddressUrl,
    string City,
    string Bio,
    string ProfilePictureUrl,
    decimal Rating
    );
    
}
