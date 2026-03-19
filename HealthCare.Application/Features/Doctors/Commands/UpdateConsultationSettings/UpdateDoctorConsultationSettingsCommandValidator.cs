using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Doctors.Commands.UpdateConsultationSettings;

public class UpdateDoctorConsultationSettingsCommandValidator : AbstractValidator<UpdateDoctorConsultationSettingsCommand>
{
    public UpdateDoctorConsultationSettingsCommandValidator()
    {
        RuleFor(x => x.ClinicFee)
            .NotNull()
            .GreaterThanOrEqualTo(0)
            .WithMessage("Clinic fee cannot be negative.");

        RuleFor(x => x.HomeFee)
            .NotNull()
            .GreaterThanOrEqualTo(0)
            .WithMessage("Home visit fee cannot be negative.");

        RuleFor(x => x.OnlineFee)
            .NotNull()
            .GreaterThanOrEqualTo(0)
            .WithMessage("Online fee cannot be negative.");



        When(x => x.AllowOnlineConsultation, () => {
            RuleFor(x => x.OnlineFee)
                .GreaterThan(0)
                .WithMessage("Please set a fee for online consultations since you have enabled them.");
        });

        When(x => x.AllowHomeVisit, () => {
            RuleFor(x => x.HomeFee)
                .GreaterThan(0)
                .WithMessage("Please set a fee for home visits since you have enabled them.");
        });
    }
}
