using HealthCare.Application.Common.Helpers;
using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Chats.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Communication.Commands.SendMessage;

public class SendMessageCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<SendMessageCommand, Result<MessageDto>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<MessageDto>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var isChatExist = await _unitOfWork.Chats.AsQueryable()
            .AnyAsync(c => c.Id == request.ChatId &&
            (c.Doctor.UserId == request.UserId || c.Patient.UserId == request.UserId), cancellationToken);

        if (!isChatExist)
            return Result.Failure<MessageDto>(new Error("ChatErrors.NotExistOrUnauthorize", "Chat not found or user is not a member of the chat.", 404));

        var message = new Message
        {
            ChatId = request.ChatId,
            SenderId = request.UserId,
            Content = request.Message,
            CreatedAt = DateTime.UtcNow.GetEgyptTime()
        };

        await _unitOfWork.Messages.AddAsync(message, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var senderName = await _unitOfWork.Users.AsQueryable()
            .Where(u => u.Id == request.UserId)
            .Select(u => u.Name)
            .FirstOrDefaultAsync(cancellationToken);

        var response = new MessageDto(
            message.Id,
            message.SenderId,
            senderName!,
            message.Content,
            message.CreatedAt
        );

        return Result.Success(response);
    }
}
