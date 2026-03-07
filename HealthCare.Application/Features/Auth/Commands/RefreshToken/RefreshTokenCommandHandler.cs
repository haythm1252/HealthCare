using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Auth.Contracts;
using HealthCare.Application.Services;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HealthCare.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler(IAuthService authService) : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    private readonly IAuthService _authService = authService;

    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        return await _authService.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);
    }
}
