using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.NurseShifts.Contracts;

public record ShiftDto(
    Guid Id,
    TimeOnly StartTime,
    TimeOnly EndTime,
    bool IsBooked
);

