using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.NurseShifts.Commands.GenereateShifts;

public class GenerateNurseShiftsCommandValidator : AbstractValidator<GenerateNurseShiftsCommand>
{
    public GenerateNurseShiftsCommandValidator()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var nextThreeMonthes = today.AddDays(90);

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .GreaterThanOrEqualTo(today)
            .WithMessage("Start date cannot be in the past.")
            .LessThanOrEqualTo(nextThreeMonthes)
            .WithMessage($"Shifts can only be planned up to 90 days in advance (until {nextThreeMonthes}).");


        RuleFor(x => x)
            .Must(x => !x.EndDate.HasValue || x.EndDate.Value >= x.StartDate)
            .WithMessage("End date must be after or equal to start date.")
            .Must(x => !x.EndDate.HasValue || x.EndDate.Value <= x.StartDate.AddDays(30))
            .WithMessage("You can only generate shifts for a maximum of 30 days in a single request.");

        RuleFor(x => x.StartTime)
            .NotEmpty()
            .WithMessage("Start time is required.");

        RuleFor(x => x)
            .Must(x => x.EndTime > x.StartTime)
            .WithMessage("End time must be after start time in the same day.")
            .Must(x => (x.EndTime.ToTimeSpan() - x.StartTime.ToTimeSpan()).TotalHours >= 1)
            .WithMessage("A nurse shift must be at least 1 hour long.");
    }
}
