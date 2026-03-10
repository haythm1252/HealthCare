using FluentValidation;

namespace HealthCare.Application.Features.Auth.Commands.VerifyResetPasswordOtp;

public class VerifyResetPasswordOtpCommandValidator : AbstractValidator<VerifyResetPasswordOtpCommand>
{
    public VerifyResetPasswordOtpCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Otp)
            .NotEmpty();
    }
}
