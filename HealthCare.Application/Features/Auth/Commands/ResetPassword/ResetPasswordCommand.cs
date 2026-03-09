using HealthCare.Application.Common.Result;
using MediatR;

namespace HealthCare.Application.Features.Auth.Commands.ResetPassword;

public record ResetPasswordCommand(string Email, string Otp, string NewPassword) : IRequest<Result>;
