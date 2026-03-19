using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.LabTest.Commands.UpdateLabTest;

public class UpdateLabTestCommandValidator : AbstractValidator<UpdateLabTestCommand>
{
    public UpdateLabTestCommandValidator()
    {
        RuleFor(x => x.LabTestId)
            .NotEmpty();

        RuleFor(x => x.Price)
            .NotEmpty()
            .GreaterThanOrEqualTo(1);
    }
}
