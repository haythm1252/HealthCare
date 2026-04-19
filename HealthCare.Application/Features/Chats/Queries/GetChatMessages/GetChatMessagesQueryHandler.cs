using HealthCare.Application.Common.Pagination;
using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Chats.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Chats.Queries.GetChatMessages;


public class GetChatMessagesQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetChatMessagesQuery, Result<PagedList<MessageDto>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<PagedList<MessageDto>>> Handle(GetChatMessagesQuery request, CancellationToken cancellationToken)
    {
        var isUserInChat = await _unitOfWork.Chats.AnyAsync(c => c.Id == request.ChatId && 
            (c.Patient.UserId == request.UserId || c.Doctor.UserId == request.UserId), cancellationToken);

        if (!isUserInChat)
            return Result.Failure<PagedList<MessageDto>>(new Error(
                "ChatError.NotFoundOrUnauthorize", "Chat not found or you are not authorized to access it.", 404));

        var messages = await _unitOfWork.Messages.AsQueryable()
            .Where(m => m.ChatId == request.ChatId)
            .OrderByDescending(m => m.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .OrderBy(m => m.CreatedAt) // make it in opposite order so the new messages will be in the end 
            .Select(m => new MessageDto
            (
                m.Id,
                m.SenderId,
                m.Sender.Name,
                m.Content,
                m.CreatedAt
            ))
            .ToListAsync(cancellationToken);

        var count = await _unitOfWork.Messages.AsQueryable()
            .Where(m => m.ChatId == request.ChatId)
            .CountAsync(cancellationToken);


        var pagedMessages = new PagedList<MessageDto>(messages, request.Page, count, request.PageSize);

        return Result.Success(pagedMessages);
    }
}
