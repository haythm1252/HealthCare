using HealthCare.Application.Common.Result;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Communication.Commands.JoinChat;

public class JoinChatCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<JoinChatCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(JoinChatCommand request, CancellationToken cancellationToken)
    {
        var isChatExist = await _unitOfWork.Chats.AsQueryable()
            .AnyAsync(c => c.Id == request.ChatId && 
            (c.Doctor.UserId == request.UserId || c.Patient.UserId == request.UserId), cancellationToken);

        if(!isChatExist)
            return Result.Failure(new Error("ChatErrors.NotExistOrUnauthorize","Chat not found or user is not a member of the chat.", 404));

        return Result.Success();
    }
}
