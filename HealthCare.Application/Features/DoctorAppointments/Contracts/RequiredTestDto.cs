using HealthCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.DoctorAppointments.Contracts;

public record RequiredTestDto(
    Guid TestId,
    string TestName,
    TestResultStatus Status
);
