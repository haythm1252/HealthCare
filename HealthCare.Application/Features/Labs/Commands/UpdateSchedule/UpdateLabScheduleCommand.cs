using HealthCare.Application.Common.Result;
using MediatR;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace HealthCare.Application.Features.Labs.Commands.UpdateSchedule;

public record UpdateLabScheduleCommand(
    string UserId,
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
) : IRequest<Result>;
