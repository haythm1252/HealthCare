using FluentValidation;
using HealthCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.LabAppointment.Queries.GetLabAppointments;

public class GetLabAppointmentsQueryValidator : AbstractValidator<GetLabAppointmentsQuery>
{
    public GetLabAppointmentsQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Page)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50)
            .WithMessage("Page size must be between 1 and 50.");

        RuleFor(x => x.Search)
            .MaximumLength(100)
            .WithMessage("Search term cannot exceed 100 characters.");

        RuleFor(x => x.Status)
            .IsEnumName(typeof(AppointmentStatus), caseSensitive: false)
            .When(x => !string.IsNullOrEmpty(x.Status))
            .WithMessage("Invalid Appointment Status.");

        RuleFor(x => x.AppointmentType)
            .IsEnumName(typeof(AppointmentType), caseSensitive: false)
            .WithMessage($"Invalid appointment type. Please enter {AppointmentType.HomeVisit} or {AppointmentType.OnSiteVisit}.")
            .NotEqual(AppointmentType.Online.ToString(), StringComparer.OrdinalIgnoreCase)
            .WithMessage("Online appointments are not supported for laboratory services.");
    }
}
