using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Auth.Contracts;
using HealthCare.Application.Services;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HealthCare.Application.Features.Auth.Commands.VerifyResetPasswordOtp;

public class VerifyResetPasswordOtpCommandHandler(IAuthService authService) : IRequestHandler<VerifyResetPasswordOtpCommand, Result>
{
    private readonly IAuthService _authService = authService;

    public async Task<Result> Handle(VerifyResetPasswordOtpCommand request, CancellationToken cancellationToken)
    {
        return await _authService.VerifyResetPasswordOtpAsync(request.Email, request.Otp, cancellationToken);
    }
}
