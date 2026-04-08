using HealthCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Patients.MedicalRecordContracts;

public record TestResultDto(
    Guid Id,
    string TestName,
    decimal? Value,
    string? Summary,
    string ResultFileUrl,
    TestResultStatus Status,
    DateTime? SubmittedAt
);
