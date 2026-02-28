using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Auth.Contracts;

public record DiseasesDto
(
    bool HasDiabetes,
    bool HasBloodPressure,
    bool HasAsthma,
    bool HasHeartDisease,
    bool HasKidneyDisease,
    bool HasArthritis,
    bool HasCancer,
    bool HasHighCholesterol,
    string? OtherMedicalConditions
);
