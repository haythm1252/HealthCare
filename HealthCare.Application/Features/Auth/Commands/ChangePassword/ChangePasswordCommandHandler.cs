using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Auth.Contracts;
using HealthCare.Application.Services;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HealthCare.Application.Features.Auth.Commands.ChangePassword;

public class ChangePasswordCommandHandler(IAuthService authService) : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly IAuthService _authService = authService;

    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        return await _authService.ChangePasswordAsync(request.UserId, request.CurrentPassword, request.NewPassword, cancellationToken);
    }
}
