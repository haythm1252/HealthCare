using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Tests.Commands.AddTest;

public class AddTestCommandValidator :AbstractValidator<AddTestCommand>
{
    public AddTestCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(x => x.PreRequisites)
            .NotEmpty()
            .MaximumLength(250);
    }
}
