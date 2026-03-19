using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Nurses.Commands.UpdatePricing;

public class UpdateNursePricingCommandValidator :AbstractValidator<UpdateNursePricingCommand>
{
    public UpdateNursePricingCommandValidator()
    {
        RuleFor(x => x.HomeVisitFee)
            .NotNull()
            .GreaterThanOrEqualTo(0)
            .WithMessage("Home visit fee cannot be negative.");

        RuleFor(x => x.HourPrice)
            .NotNull()
            .GreaterThanOrEqualTo(0)
            .WithMessage("HourPrice cannot be negative.");
    }
}
