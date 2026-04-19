using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Chats.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Chats.Queries.GetChats;

public class GetChatsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetChatsQuery, IEnumerable<ChatDto>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IEnumerable<ChatDto>> Handle(GetChatsQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Chats.AsQueryable()
            .Where(c => c.Patient.UserId == request.UserId || c.Doctor.UserId == request.UserId)
            .Select(c => new
            {
                Chat = c,
                LastMsg = c.Messages.OrderByDescending(m => m.CreatedAt).FirstOrDefault(),
                IsDoctor = c.Doctor.UserId == request.UserId
            })
            .Select(c => new ChatDto
            (
                c.Chat.Id,
                c.IsDoctor ? c.Chat.Patient.User.Name : c.Chat.Doctor.User.Name,
                c.IsDoctor ? null : c.Chat.Doctor.ProfilePictureUrl,

                c.LastMsg != null ?  c.LastMsg.Content : null ,
                c.LastMsg != null ?  c.LastMsg.CreatedAt : null,
                c.LastMsg != null ? c.LastMsg.SenderId == request.UserId : false
            ))
            .ToListAsync(cancellationToken);
    }
}
