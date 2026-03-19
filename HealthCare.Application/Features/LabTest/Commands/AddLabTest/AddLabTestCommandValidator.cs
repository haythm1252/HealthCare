using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.LabTest.Commands.AddLabTest;

public class AddLabTestCommandValidator : AbstractValidator<AddLabTestCommand>
{
    public AddLabTestCommandValidator()
    {
        RuleFor(x => x.TestId)
            .NotEmpty();

        RuleFor(x => x.Price)
            .NotEmpty()
            .GreaterThanOrEqualTo(1);
    }
}
