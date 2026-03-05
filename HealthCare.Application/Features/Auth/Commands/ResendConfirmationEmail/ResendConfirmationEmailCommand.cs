using HealthCare.Application.Common.Result;
using MediatR;

namespace HealthCare.Application.Features.Auth.Commands.ResendConfirmationEmail;

public record ResendConfirmationEmailCommand(string Email, string CallbackUrl) : IRequest<Result>;
