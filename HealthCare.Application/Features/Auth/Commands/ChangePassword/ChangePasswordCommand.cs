using HealthCare.Application.Common.Result;
using MediatR;

namespace HealthCare.Application.Features.Auth.Commands.ChangePassword;

public record ChangePasswordCommand(string UserId, string CurrentPassword, string NewPassword) : IRequest<Result>;
