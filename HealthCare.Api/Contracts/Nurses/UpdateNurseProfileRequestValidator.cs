using FluentValidation;
using HealthCare.Application.Common.Consts;
using HealthCare.Application.Common.Helpers;

namespace HealthCare.Api.Contracts.Nurses;

public class UpdateNurseProfileRequestValidator : AbstractValidator<UpdateNurseProfileRequest>
{
    public UpdateNurseProfileRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(RegexPatterns.PhoneNumber)
            .WithMessage("Phone number must be a valid Egyptian mobile number.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required.")
            .MaximumLength(200).WithMessage("Address must not exceed 200 characters.");

        RuleFor(x => x.City)
            .NotEmpty()
            .Must(EgyptGovernorates.IsValid)
            .WithMessage("Invalid governorate, Please select a valid governorate.");

        RuleFor(x => x.ProfilePicture!)
            .SetValidator(new ImageValidator()).When(x => x.ProfilePicture is not null);
    }
}
