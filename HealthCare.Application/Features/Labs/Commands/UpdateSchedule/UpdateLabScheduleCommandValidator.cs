using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Labs.Commands.UpdateSchedule;

public class UpdateLabScheduleCommandValidator : AbstractValidator<UpdateLabScheduleCommand>
{
    public UpdateLabScheduleCommandValidator()
    {
        RuleFor(x => x.HomeVisitFee)
            .NotEmpty();

        RuleFor(x => x.OpeningTime)
            .NotEmpty();

        RuleFor(x => x.ClosingTime)
            .NotEmpty()
            .GreaterThan(x => x.OpeningTime).WithMessage("closing time should be after the opening time");
            
    }
}
