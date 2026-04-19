using FluentValidation;
using HealthCare.Application.Common.Consts;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Chats.Queries.GetChats;

public class GetChatsQueryValidator : AbstractValidator<GetChatsQuery>
{
    public GetChatsQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}