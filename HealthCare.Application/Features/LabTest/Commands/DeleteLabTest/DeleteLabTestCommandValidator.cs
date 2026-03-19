using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.LabTest.Commands.DeleteLabTest;

public class DeleteLabTestCommandValidator : AbstractValidator<DeleteLabTestCommand>
{
    public DeleteLabTestCommandValidator()
    {
        RuleFor(x => x.LabTestId)
            .NotEmpty();
    }
}
