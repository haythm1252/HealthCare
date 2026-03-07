using HealthCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Patients.Contracts;

public record PatientProfileResponse(
    string Id,
    string Name,
    Gender Gender,
    string Email,
    string PhoneNumber,
    string Address,
    string? AddressUrl,
    string City,

    DateOnly DateOfBirth,
    bool HasDiabetes,
    bool HasBloodPressure,
    bool HasAsthma,
    bool HasHeartDisease,
    bool HasKidneyDisease,
    bool HasArthritis,
    bool HasCancer,
    bool HasHighCholesterol,
    string? OtherMedicalConditions,
    decimal? Weight 
);