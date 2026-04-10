using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Specialities.Commands.UpdateSpeciality;

public class UpdateSpecialityCommandValidator : AbstractValidator<UpdateSpecialityCommand>
{
    public UpdateSpecialityCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);
    }
}
