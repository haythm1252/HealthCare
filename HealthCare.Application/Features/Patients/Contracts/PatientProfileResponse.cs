using HealthCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Patients.Contracts;

public record PatientProfileResponse(
    string Name,
    string Email,
    string PhoneNumber,
    string Address,
    string City,
    string? AddressUrl,
    Gender Gender,
    bool HasKidneyDisease,
    bool HasArthritis ,
    bool HasCancer
);
