using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Labs.Contracts;

public record UpdateLabProfileRequest(
    string Name,
    string Bio,
    string PhoneNumber,
    string Address,
    string? AddressUrl,
    string City,
    IFormFile? ProfilePicture
);

