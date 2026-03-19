using HealthCare.Application.Features.NurseShifts.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Nurses.Contracts;

public record NurseScheduleResponse(
    decimal HomeVisitFee,
    decimal HourPrice,
    IEnumerable<DailyShiftsDto> Shifts
);  

