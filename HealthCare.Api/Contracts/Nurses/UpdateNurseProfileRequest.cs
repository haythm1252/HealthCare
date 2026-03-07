namespace HealthCare.Api.Contracts.Nurses;

public record UpdateNurseProfileRequest(
    string Name,
    string Bio,
    string PhoneNumber,
    string Address,
    string? AddressUrl,
    string City,
    IFormFile? ProfilePicture
);
