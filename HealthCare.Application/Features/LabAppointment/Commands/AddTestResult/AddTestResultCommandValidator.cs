using FluentValidation;
using HealthCare.Application.Common.Consts;
using Microsoft.AspNetCore.Http;

namespace HealthCare.Application.Features.LabAppointment.Commands.AddTestResult;

public class AddTestResultCommandValidator : AbstractValidator<AddTestResultCommand>
{
    public AddTestResultCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.AppointmentId)
            .NotEmpty();

        RuleFor(x => x.TestResultId)
            .NotEmpty();

        RuleFor(x => x.Value)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Test value cannot be negative.");

        RuleFor(x => x.Summary)
            .MaximumLength(1000);

        RuleFor(x => x.ResultFile)
            .Must(BeAPdf!)
            .WithMessage("Only PDF files are allowed.")
            .Must(HaveValidSize!)
            .WithMessage($"File size must not exceed {FileSettings.MaxPdfSizeInMb} MB.")
            .When(x => x.ResultFile != null);
    }

    private bool BeAPdf(IFormFile file)
    {
        if (file == null) return false;

        return file.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase) ||
               Path.GetExtension(file.FileName).Equals(".pdf", StringComparison.OrdinalIgnoreCase);
    }

    private bool HaveValidSize(IFormFile file)
    {
        if (file == null) return false;

        return file.Length <= FileSettings.MaxPdfSizeInBytes;
    }
}
