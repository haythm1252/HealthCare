using HealthCare.Application.Common.Pagination;
using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Chats.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Chats.Queries.GetChatMessages;

public record GetChatMessagesQuery(
    string UserId,
    Guid ChatId,
    int Page = 1,
    int PageSize = 20
) : IRequest<Result<PagedList<MessageDto>>>;
