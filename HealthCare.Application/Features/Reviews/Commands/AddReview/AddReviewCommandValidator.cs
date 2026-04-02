using FluentValidation;
using HealthCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Reviews.Commands.AddReview;

public class AddReviewCommandValidator : AbstractValidator<AddReviewCommand>
{
    public AddReviewCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.TargetId)
            .NotEmpty();

        RuleFor(x => x.TargetType)
            .NotEmpty()
            .IsEnumName(typeof(TargetType), caseSensitive: false)
            .WithMessage($"Invalid Target type. Please enter {TargetType.Doctor} or {TargetType.Nurse} or {TargetType.Lab}.");

        RuleFor(x => x.Rating)
            .NotEmpty()
            .InclusiveBetween(1, 5)
            .WithMessage("The rating value is from 1 to 5");

        RuleFor(x => x.Comment)
            .MaximumLength(500)
            .WithMessage("The comment must not exceed 500 characters.");

    }
}
