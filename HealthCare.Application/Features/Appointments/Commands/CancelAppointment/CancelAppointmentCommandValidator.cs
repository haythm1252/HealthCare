using FluentValidation;
using HealthCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Appointments.Commands.CancelAppointment;

public class CancelAppointmentCommandValidator : AbstractValidator<CancelAppointmentCommand>
{
    public CancelAppointmentCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.AppointmentId)
            .NotEmpty();

        RuleFor(x => x.AppointmentType)
            .NotEmpty()
            .IsEnumName(typeof(TargetType), caseSensitive: false)
            .WithMessage($"Invalid AppointmentType type. Please enter {TargetType.Doctor} or {TargetType.Nurse} or {TargetType.Lab}.");
    }
}
