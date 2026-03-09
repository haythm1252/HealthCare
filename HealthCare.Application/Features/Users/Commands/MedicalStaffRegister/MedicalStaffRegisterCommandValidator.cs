using FluentValidation;
using HealthCare.Application.Common.Consts;
using HealthCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace HealthCare.Application.Features.Users.Commands.MedicalStaffRegister;

public class MedicalStaffRegisterCommandValidator : AbstractValidator<MedicalStaffRegisterCommand>
{
    public MedicalStaffRegisterCommandValidator()
    {

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

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

        RuleFor(x => x.Role)
            .NotEmpty()
            .Must(role => DefaultRoles.MedicalStaffRoles.Contains(role))
            .WithMessage($"Role must be one of the following: {string.Join(", ", DefaultRoles.MedicalStaffRoles)} (case sensitive).");

        RuleFor(x => x.SpecialityId)
            .NotEmpty()
            .When(x => x.Role == DefaultRoles.Doctor)
            .WithMessage("SpecialityId is required for doctors.");

        RuleFor(x => x.SpecialityId)
            .Null()
            .When(x => x.Role != DefaultRoles.Doctor)
            .WithMessage("SpecialityId should not be provided for non-doctors, Only doctors can have a Speciality.");
    }



}
