using HealthCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Patients.Contracts;

public record UpdatePatientProfileRequest(
    string Name,
    string PhoneNumber,
    string Address,
    string? AddressUrl,
    string City,
    decimal? Weight
);
