using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Nurses.Contracts;

public record NurseResponse
(
    Guid Id,
    string Name,
    string City,
    decimal VisitFee,
    decimal HourPrice,
    decimal Rating,
    int RatingsCount,
    string ProfilePictureUrl
);
