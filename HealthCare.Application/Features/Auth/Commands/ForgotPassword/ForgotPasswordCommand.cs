using HealthCare.Application.Common.Result;
using MediatR;

namespace HealthCare.Application.Features.Auth.Commands.ForgotPassword;

public record ForgotPasswordCommand(string Email) : IRequest<Result>;
