using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.AiChatBot.Contracts;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.AiChatBot.Commands;

public record SendAiChatCommand(
    string UserId,
    string Message,
    IFormFile? Attachment
) : IRequest<Result<AiChatResponse>>;
