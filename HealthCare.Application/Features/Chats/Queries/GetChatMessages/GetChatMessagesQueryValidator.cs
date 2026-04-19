using FluentValidation;
using HealthCare.Application.Common.Consts;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Chats.Queries.GetChatMessages;

public class GetChatMessagesQueryValidator : AbstractValidator<GetChatMessagesQuery>
{
    public GetChatMessagesQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.ChatId)
            .NotEmpty();

        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50)
            .WithMessage("PageSize must be between 1 and 50.");
    }
}
