using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Doctors.Contracts;

public record DoctorResponse(
    Guid Id,
    string Name,
    string Specialty,
    string Title,
    string Address,
    decimal Fee,
    decimal Rating,
    int RatingsCount,
    bool AllowHome,
    bool AllowOnline,
    string ProfilePictureUrl

);
