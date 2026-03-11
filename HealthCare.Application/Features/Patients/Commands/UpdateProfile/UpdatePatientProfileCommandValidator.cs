using FluentValidation;
using HealthCare.Application.Common.Consts;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Patients.Commands.UpdateProfile;

public class UpdatePatientProfileCommandValidator : AbstractValidator<UpdatePatientProfileCommand>
{
    public UpdatePatientProfileCommandValidator()
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

        RuleFor(x => x.Weight)
            .GreaterThan(0).When(x => x.Weight.HasValue).WithMessage("Weight must be greater than 0.")
            .LessThan(300).When(x => x.Weight.HasValue).WithMessage("Weight must be less than 300 kg.");
    }
}
