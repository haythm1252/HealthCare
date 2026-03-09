using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Auth.Contracts;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HealthCare.Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordCommandHandler(IAuthService authService) : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IAuthService _authService = authService;

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        return await _authService.ResetPasswordAsync(request.Email, request.Otp, request.NewPassword, cancellationToken);
    }
}
