using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Auth.Contracts;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HealthCare.Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler(IAuthService authService) : IRequestHandler<ForgotPasswordCommand, Result>
{
    private readonly IAuthService _authService = authService;

    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        return await _authService.ForgotPasswordAsync(request.Email, cancellationToken);
    }
}
