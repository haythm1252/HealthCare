using HealthCare.Application.Common.Result;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Nurses.Commands.UpdatePricing;

public record UpdateNursePricingCommand(
    string UserId,
    decimal HomeVisitFee,
    decimal HourPrice
) : IRequest<Result>;
