using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.DoctorAppointments.Commands.AddDiagnosis;

public class AddDiagnosisCommandValidator : AbstractValidator<AddDiagnosisCommand>
{
    public AddDiagnosisCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.AppointmentId)
            .NotEmpty();

        RuleFor(x => x.Diagnosis)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.Prescriptions)
            .NotEmpty()
            .MaximumLength(700);

        RuleFor(x => x.RequiredTests)
            .Must(x => x!.Distinct().Count() == x!.Count())
            .WithMessage("The list of required tests contains duplicate entries.") 
            .When(x => x.RequiredTests is not null && x.RequiredTests.Any());

    }
}
