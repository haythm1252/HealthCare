using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.NurseShifts.Contracts;

public record GenerateNurseShiftsRequest(
    DateOnly StartDate,
    DateOnly? EndDate,
    TimeOnly StartTime,
    TimeOnly EndTime
);
