using FluentValidation;
using HealthCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.LabAppointment.Commands.BookLabAppointment;

public class BookLabAppointmentCommandValidator : AbstractValidator<BookLabAppointmentCommand>
{
    public BookLabAppointmentCommandValidator()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var nextThreeMonths = today.AddDays(90);

        RuleFor(x => x.LabId)
            .NotEmpty();

        RuleFor(x => x.Date)
            .NotEmpty()
            .GreaterThanOrEqualTo(today)
            .WithMessage("Appointment date cannot be in the past.")
            .LessThanOrEqualTo(nextThreeMonths)
            .WithMessage("Appointments can only be booked up to 90 days in advance.");

        RuleFor(x => x.AppointmentType)
            .NotEmpty()
            .WithMessage("Appointment type is required.")
            .IsEnumName(typeof(AppointmentType), caseSensitive: false)
            .WithMessage($"Invalid appointment type. Please enter {AppointmentType.HomeVisit} or {AppointmentType.OnSiteVisit}.")
            .NotEqual(AppointmentType.Online.ToString(), StringComparer.OrdinalIgnoreCase)
            .WithMessage("Online appointments are not supported for laboratory services.");

        RuleFor(x => x.Address)
            .NotEmpty()
            .When(x => x.AppointmentType.Equals(AppointmentType.HomeVisit.ToString(), StringComparison.OrdinalIgnoreCase))
            .WithMessage("Home address is required for Home Visit appointments.");

        RuleFor(x => x.LabTestsIds)
            .NotEmpty()
            .WithMessage("You must select at least one lab test to book an appointment.")
            .Must(BeNotDublicated)
            .WithMessage("the tests added are dublicated you send the same test more than one time");
    }

    private bool BeNotDublicated(IEnumerable<Guid> LabTestsIds)
    {
        if (LabTestsIds == null) return true; // Let NotEmpty handle the error message

        return LabTestsIds.Count() == LabTestsIds.Distinct().Count();
    }
}
