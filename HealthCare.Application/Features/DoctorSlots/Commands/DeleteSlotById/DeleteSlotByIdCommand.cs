using HealthCare.Application.Common.Result;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.DoctorSlots.Commands.DeleteSlotsById;

public record DeleteSlotByIdCommand(
    string UserId,
    Guid SlotId
) : IRequest<Result>;