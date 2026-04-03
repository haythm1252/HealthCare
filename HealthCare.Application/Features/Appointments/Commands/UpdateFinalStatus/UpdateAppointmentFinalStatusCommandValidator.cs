using FluentValidation;
using HealthCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Appointments.Commands.UpdateFinalStatus;

public class UpdateAppointmentFinalStatusCommandValidator : AbstractValidator<UpdateAppointmentFinalStatusCommand>
{
    public UpdateAppointmentFinalStatusCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.UserRole)
            .NotEmpty();

        RuleFor(x => x.AppointmentId)
            .NotEmpty();

        RuleFor(x => x.AppointmentType)
            .NotEmpty()
            .IsEnumName(typeof(TargetType), caseSensitive: false)
            .WithMessage($"Invalid Target type. Please enter {TargetType.Doctor} or {TargetType.Nurse} or {TargetType.Lab}.");

        RuleFor(x => x.Status)
            .Must(x => x == AppointmentStatus.Completed.ToString() || x == AppointmentStatus.NoShow.ToString())
            .WithMessage("Invalid status you have to chose Completed or NoShow.");

        RuleFor(x => x)
            .Must(x => x.UserRole.Equals(x.AppointmentType, StringComparison.OrdinalIgnoreCase))
            .WithMessage("Access Denied: Your account role does not match this appointment category.").WithName("role");
    }
}
