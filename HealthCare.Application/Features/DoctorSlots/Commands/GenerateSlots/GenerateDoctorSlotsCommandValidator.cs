using FluentValidation;
using HealthCare.Application.Features.DoctorSlots.Commands.GenerateSlots;

public class GenerateDoctorSlotsCommandValidator : AbstractValidator<GenerateDoctorSlotsCommand>
{
    public GenerateDoctorSlotsCommandValidator()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var nextThreeMonths = today.AddDays(90);

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .GreaterThanOrEqualTo(today)
            .WithMessage("Start date cannot be in the past.")
            .LessThanOrEqualTo(nextThreeMonths)
            .WithMessage("You can only plan your schedule up to 90 days in advance.");

        RuleFor(x => x)
            .Must(x => !x.EndDate.HasValue || x.EndDate.Value >= x.StartDate)
            .WithMessage("End date must be after or equal to start date.")
            .Must(x => !x.EndDate.HasValue || x.EndDate.Value <= x.StartDate.AddDays(30))
            .WithMessage("You can only generate slots for a maximum of 30 days in a single request.");

        RuleFor(x => x.StartTime)
            .NotEmpty()
            .WithMessage("Start time is required.");

        RuleFor(x => x)
            .Must(x => !x.EndTime.HasValue || x.EndTime.Value > x.StartTime)
            .WithMessage("End time must be after start time.");

        RuleFor(x => x.ConsultationDurationInminutes)
            .InclusiveBetween(5, 60)
            .WithMessage("Consultation must be between 5 and 60 minutes.");
    }
}