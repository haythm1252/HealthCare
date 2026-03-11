using FluentValidation;
using HealthCare.Application.Common.Consts;
using HealthCare.Application.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Doctors.Commands.UpdateProfile;

public class UpdateDoctorProfileCommandValidator : AbstractValidator<UpdateDoctorProfileCommand>
{
    public UpdateDoctorProfileCommandValidator()
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


        RuleFor(x => x.Bio)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);   

        RuleFor(x => x.ProfilePicture!)
            .SetValidator(new ImageValidator()).When(x => x.ProfilePicture is not null);
    }
}
