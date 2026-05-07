using FluentValidation;
using HealthCare.Application.Common.Consts;
using HealthCare.Application.Features.AiChatBot.Commands.Gemini;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.AiChatBot.Commands.MedGemma;

public class SendMedGemmaChatCommandValidator : AbstractValidator<SendMedGemmaChatCommand>
{
    public SendMedGemmaChatCommandValidator()
    {
        RuleFor(x => x.Message)
            .NotNull()
            .MaximumLength(600);

    }

    

}
