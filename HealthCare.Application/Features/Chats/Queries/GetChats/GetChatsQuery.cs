using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Chats.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Chats.Queries.GetChats;

public record GetChatsQuery(string UserId) : IRequest<IEnumerable<ChatDto>>;
