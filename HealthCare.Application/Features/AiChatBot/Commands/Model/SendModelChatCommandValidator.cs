using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.AiChatBot.Commands.Model;

public class SendModelChatCommandValidator : AbstractValidator<SendModelChatCommand>
{
    public SendModelChatCommandValidator()
    {

        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Message)
            .NotEmpty()
            .MaximumLength(600);
    }
}
