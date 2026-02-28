using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Auth.Contracts;
using HealthCare.Application.Services;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HealthCare.Application.Features.Auth.Commands.ConfirmEmail;

public class ConfirmEmailCommandHandler(IAuthService authService) : IRequestHandler<ConfirmEmailCommand, Result>
{
    private readonly IAuthService _authService = authService;

    public async Task<Result> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        return await _authService.ConfirmEmailAsync(request.UserId, request.Token, cancellationToken);
    }
}
