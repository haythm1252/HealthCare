using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Specialities.Contracts;

public record SpecialityResponse(
    Guid Id,
    string Name
);
