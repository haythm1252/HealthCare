using FluentValidation;
using HealthCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Reviews.Queries.GetReviews;

public class GetReviewsQueryValidator : AbstractValidator<GetReviewsQuery>
{
    public GetReviewsQueryValidator()
    {

        RuleFor(x => x.TargetId)
            .NotEmpty();

        RuleFor(x => x.TargetType)
            .NotEmpty()
            .IsEnumName(typeof(TargetType), caseSensitive: false)
            .WithMessage($"Invalid Target type. Please enter {TargetType.Doctor} or {TargetType.Nurse} or {TargetType.Lab}.");

        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50)
            .WithMessage("PageSize must be between 1 and 50.");

        RuleFor(x => x)
            .Must(x => !(x.SortByDate && x.SortByRating))
            .WithMessage("Cannot sort by both date and rating at the same time.");

    }
}
