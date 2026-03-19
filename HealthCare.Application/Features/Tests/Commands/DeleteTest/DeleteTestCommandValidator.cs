using FluentValidation;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Tests.Commands.DeleteTest;

public class DeleteTestCommandValidator : AbstractValidator<DeleteTestCommand>
{
    public DeleteTestCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .Must(x => x != Guid.Empty);
    }
}
