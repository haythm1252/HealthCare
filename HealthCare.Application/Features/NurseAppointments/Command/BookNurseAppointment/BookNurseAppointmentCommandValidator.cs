using FluentValidation;
using HealthCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.NurseAppointments.Command.BookNurseAppointment;

public class BookNurseAppointmentCommandValidator : AbstractValidator<BookNurseAppointmentCommand>
{
    public BookNurseAppointmentCommandValidator()
    {
        RuleFor(x => x.NurseId)
            .NotEmpty();

        RuleFor(x => x.ShiftId)
            .NotEmpty();

        RuleFor(x => x.Address)
            .NotEmpty()
            .MaximumLength(250);

        RuleFor(x => x.ServiceType)
            .NotEmpty().WithMessage("Service type is required.")
            .IsEnumName(typeof(NurseServiceType), caseSensitive: false)
            .WithMessage("Invalid service type. Please choose 'QuickVisit' or 'HourlyStay'.");

        RuleFor(x => x.StartTime)
            .NotEmpty();

        RuleFor(x => x.Hours)
            .NotEmpty()
            .GreaterThan(0)
            .When(x => x.ServiceType.Equals(NurseServiceType.HourlyStay.ToString(), StringComparison.OrdinalIgnoreCase) == true)
            .WithMessage("Hours must be greater than 0 when 'HourlyStay' is selected.");
    }
}
