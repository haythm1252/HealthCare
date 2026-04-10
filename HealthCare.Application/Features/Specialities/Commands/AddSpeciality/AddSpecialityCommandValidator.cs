using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Specialities.Commands.AddSpeciality;

public class AddSpecialityCommandValidator : AbstractValidator<AddSpecialityCommand>
{
    public AddSpecialityCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);
    }
}
