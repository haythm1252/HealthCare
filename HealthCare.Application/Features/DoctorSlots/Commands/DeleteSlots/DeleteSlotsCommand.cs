using HealthCare.Application.Common.Result;
using MediatR;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.DoctorSlots.Commands.DeleteSlots;

public record DeleteSlotsCommand(
    string UserId,
    DateOnly Date
) : IRequest<Result>;
