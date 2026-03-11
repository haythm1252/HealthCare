using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Doctors.Contracts;

public record UpdateDoctorProfileRequest(
    string Name,
    string Title,
    string Bio,
    string PhoneNumber,
    string Address,
    string? AddressUrl,
    string City,
    IFormFile? ProfilePicture
);
