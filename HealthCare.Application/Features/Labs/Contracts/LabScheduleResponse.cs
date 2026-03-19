using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Labs.Contracts;

public record LabScheduleResponse(
    decimal HomeVisitFee,
    TimeOnly OpeningTime,
    TimeOnly ClosingTime,
    bool IsSaturdayOpen,
    bool IsSundayOpen,
    bool IsMondayOpen,
    bool IsTuesdayOpen,
    bool IsWednesdayOpen,
    bool IsThursdayOpen, 
    bool IsFridayOpen
);