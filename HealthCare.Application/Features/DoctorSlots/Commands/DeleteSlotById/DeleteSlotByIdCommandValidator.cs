using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.DoctorSlots.Commands.DeleteSlotsById;

public class DeleteSlotByIdCommandValidator : AbstractValidator<DeleteSlotByIdCommand>
{
    public DeleteSlotByIdCommandValidator()
    {
        RuleFor(x => x.SlotId)
            .NotEmpty();
    }
}
