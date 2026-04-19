using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Chats.Contracts;
using HealthCare.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Communication.Commands.SendMessage;

public record SendMessageCommand(string UserId, Guid ChatId, string Message) : IRequest<Result<MessageDto>>;
