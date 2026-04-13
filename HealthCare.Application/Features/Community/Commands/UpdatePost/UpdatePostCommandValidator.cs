using FluentValidation;
using HealthCare.Application.Common.Helpers;

namespace HealthCare.Application.Features.Community.Commands.UpdatePost;

public class UpdatePostCommandValidator : AbstractValidator<UpdatePostCommand>
{
    public UpdatePostCommandValidator()
    {
        RuleFor(x => x.PostId)
            .NotEmpty().WithMessage("Post ID is required");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required")
            .MaximumLength(3000).WithMessage("Content must not exceed 3000 characters");

        RuleFor(x => x.SpecialtyId)
            .NotEmpty().WithMessage("Specialty is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.AttachmentFile!)
            .SetValidator(new ImageValidator()).When(x => x.AttachmentFile is not null);
    }
}
