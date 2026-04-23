using FluentValidation;
using HealthCare.Application.Common.Consts;
using HealthCare.Application.Features.AiChatBot.Commands.Gemini;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.AiChatBot.Commands.Olama;

public class SendOlamaChatCommandValidator : AbstractValidator<SendOlamaChatCommand>
{
    public SendOlamaChatCommandValidator()
    {
        RuleFor(x => x.Message)
            .NotNull()
            .MaximumLength(600);

    }

    

}
