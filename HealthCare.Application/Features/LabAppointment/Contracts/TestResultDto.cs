using HealthCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.LabAppointment.Contracts;

public record TestResultDto(
    Guid TestId,
    string TestName,
    decimal? Value,
    string? Summary,
    string? ResultFileUrl,
    TestResultStatus Status,
    DateTime? SubmittedAt
);
