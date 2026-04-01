using FluentValidation;
using HealthCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.DoctorAppointments.Commands.BookDoctorAppointment;

public class BookDoctorAppointmentCommandValidator : AbstractValidator<BookDoctorAppointmentCommand>
{
    public BookDoctorAppointmentCommandValidator()
    {
        RuleFor(x => x.DoctorId)
            .NotEmpty();

        RuleFor(x => x.DoctorSlotId)
            .NotEmpty();

        RuleFor(x => x.AppointmentType)
            .NotEmpty()
            .WithMessage("Appointment type is required.")
            .IsEnumName(typeof(AppointmentType), caseSensitive: false)
            .WithMessage($"Invalid appointment type. Please enter {AppointmentType.HomeVisit} or {AppointmentType.OnSiteVisit} or {AppointmentType.Online}.");

        RuleFor(x => x.Address)
            .NotEmpty()
            .When(x => x.AppointmentType.Equals(AppointmentType.HomeVisit.ToString(), StringComparison.OrdinalIgnoreCase))
            .WithMessage("Home address is required for Home Visit appointments.");

        RuleFor(x => x.Address)
            .Null()
            .When(x => x.AppointmentType.Equals(AppointmentType.Online.ToString(), StringComparison.OrdinalIgnoreCase) ||
                       x.AppointmentType.Equals(AppointmentType.OnSiteVisit.ToString(), StringComparison.OrdinalIgnoreCase))
            .WithMessage("The Address is not required for the Online and OnSiteVisit");
    }

}
