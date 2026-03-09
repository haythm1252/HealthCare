using HealthCare.Application.Common.Result;
using MediatR;

namespace HealthCare.Application.Features.Auth.Commands.ConfirmEmail;

public record ConfirmEmailCommand(string Email, string Otp) : IRequest<Result>;
