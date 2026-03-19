using HealthCare.Application.Features.Doctors.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.NurseShifts.Contracts;

public record DailyShiftsDto(
    DateOnly Date,
    string Day,
    IEnumerable<ShiftDto> Shifts
);
