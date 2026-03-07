using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Auth.Contracts;
using MediatR;

namespace HealthCare.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string Token,string RefreshToken) : IRequest<Result<AuthResponse>>;
