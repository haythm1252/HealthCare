using HealthCare.Application.Common.Result;
using MediatR;

namespace HealthCare.Application.Features.Auth.Commands.VerifyResetPasswordOtp;

public record VerifyResetPasswordOtpCommand(string Email, string Otp) : IRequest<Result>;
