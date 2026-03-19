using HealthCare.Application.Common.Result;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.NurseShifts.Commands.DeleteShiftById;

public record DeleteShiftByIdCommand(
    string UserId,
    Guid ShiftId
) : IRequest<Result>;
