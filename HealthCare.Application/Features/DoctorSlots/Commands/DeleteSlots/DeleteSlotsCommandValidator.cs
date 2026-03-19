using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.DoctorSlots.Commands.DeleteSlots;

public class DeleteSlotsCommandValidator : AbstractValidator<DeleteSlotsCommand>
{
    public DeleteSlotsCommandValidator()
    {
        RuleFor(x => x.Date)
            .NotEmpty();
    }
}
