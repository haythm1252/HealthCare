using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Auth.Contracts;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HealthCare.Application.Features.Auth.Commands.ResendConfirmationEmail;

public class ResendConfirmationEmailCommandHandler(IAuthService authService) : IRequestHandler<ResendConfirmationEmailCommand, Result>
{
    private readonly IAuthService _authService = authService;

    public async Task<Result> Handle(ResendConfirmationEmailCommand request, CancellationToken cancellationToken)
    {
        return await _authService.ResendConfirmationEmailAsync(request.Email, request.CallbackUrl, cancellationToken);
    }
}
