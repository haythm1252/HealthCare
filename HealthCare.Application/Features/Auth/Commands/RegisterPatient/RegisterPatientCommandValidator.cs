using FluentValidation;
using HealthCare.Application.Common.Consts;
using HealthCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Auth.Commands.RegisterPatient;

public class RegisterPatientCommandValidator : AbstractValidator<RegisterPatientCommand>
{
    public RegisterPatientCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        // we could make the birth patient need to be adult but maybe a mother registering her child so we will just make sure it's in the past
        RuleFor(x => x.DateOfBirth)
            .NotEmpty()
            .LessThan(DateOnly.FromDateTime(DateTime.Now)).WithMessage("Birth date must be in the past.");

        RuleFor(x => x.City)
            .NotEmpty()
            .Must(EgyptGovernorates.IsValid)
            .WithMessage("Invalid governorate, Please select a valid governorate.");

        RuleFor(x => x.Address)
            .NotEmpty()
            .MaximumLength(250).WithMessage("Address cannot exceed 250 characters.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(RegexPatterns.PhoneNumber).WithMessage("Invalid phone number.");

        RuleFor(x => x.Gender)
            .NotEmpty()
            .IsEnumName(typeof(Gender), caseSensitive: false)
            .WithMessage("Please select a valid gender (Male or Female).");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();


        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches(RegexPatterns.Password).WithMessage("Password must be at least 8 characters long and include at least one uppercase letter, one lowercase letter, one number, and one special character.");


        RuleFor(x => x.AddressUrl)
            .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute)).When(x => !string.IsNullOrWhiteSpace(x.AddressUrl))
            .WithMessage("Address URL must be a valid URL.");

        RuleFor(x => x.Diseases)
            .NotNull().WithMessage("Diseases information is required.");

        RuleFor(x => x.Diseases.OtherMedicalConditions)
            .MaximumLength(250)
            .When(x => !string.IsNullOrWhiteSpace(x.Diseases?.OtherMedicalConditions))
            .WithMessage("Other medical conditions must be 250 characters or less.");

        RuleFor(x => x.Weight)
            .GreaterThan(0).When(x => x.Weight.HasValue).WithMessage("Weight must be greater than 0.")
            .LessThan(300).When(x => x.Weight.HasValue).WithMessage("Weight must be less than 300 kg.");



    }
}
