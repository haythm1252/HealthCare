using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Communication.Commands.JoinChat;

public class JoinChatCommandValidator : AbstractValidator<JoinChatCommand>
{
    public JoinChatCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.ChatId)
            .NotEmpty();
    }
}
