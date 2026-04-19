using HealthCare.Application.Common.Result;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Communication.Commands.JoinChat;

public record JoinChatCommand(string UserId, Guid ChatId) : IRequest<Result>;
