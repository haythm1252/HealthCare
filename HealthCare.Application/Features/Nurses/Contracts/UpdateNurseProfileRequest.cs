using Microsoft.AspNetCore.Http;

namespace HealthCare.Application.Features.Nurses.Contracts;

public record UpdateNurseProfileRequest(
    string Name,
    string Bio,
    string PhoneNumber,
    string Address,
    string? AddressUrl,
    string City,
    IFormFile? ProfilePicture
);
