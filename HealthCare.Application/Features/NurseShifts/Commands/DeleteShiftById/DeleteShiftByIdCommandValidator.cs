using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.NurseShifts.Commands.DeleteShiftById;

public class DeleteShiftByIdCommandValidator : AbstractValidator<DeleteShiftByIdCommand>
{
    public DeleteShiftByIdCommandValidator()
    {
        RuleFor(x => x.ShiftId)
            .NotEmpty();
    }
}
