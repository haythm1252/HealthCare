using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.AiChatBot.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.AiChatBot.Commands.Model;

public record SendModelChatCommand(
    string UserId,
    string Message
) : IRequest<Result<AiChatResponse>>;
