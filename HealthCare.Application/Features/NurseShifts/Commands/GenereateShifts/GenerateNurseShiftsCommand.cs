using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.NurseShifts.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.NurseShifts.Commands.GenereateShifts;

public record GenerateNurseShiftsCommand(
    string UserId,
    DateOnly StartDate,
    DateOnly? EndDate,
    TimeOnly StartTime,
    TimeOnly EndTime
) : IRequest<Result<IEnumerable<DailyShiftsDto>>>;
