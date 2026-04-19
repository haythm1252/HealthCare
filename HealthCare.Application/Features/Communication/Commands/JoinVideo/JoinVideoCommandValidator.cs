using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Communication.Commands.JoinVideo;

public class JoinVideoCommandValidator : AbstractValidator<JoinVideoCommand>
{
    public JoinVideoCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.AppointmentId)
            .NotEmpty();
    }
}
