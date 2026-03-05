using HealthCare.Application.Common.Result;
using MediatR;

namespace HealthCare.Application.Features.Auth.Commands.ResetPassword;

public record ResetPasswordCommand(string UserId, string Token, string NewPassword, string ConfirmPassword) : IRequest<Result>;
