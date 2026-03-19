using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.DoctorSlots.Contracts;

public record SlotDto(
    Guid Id,
    TimeOnly StartTime,
    TimeOnly EndTime,
    bool IsBooked
);
