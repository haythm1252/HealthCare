using HealthCare.Api.Extentions;
using HealthCare.Application.Features.Auth.Commands.Login;
using HealthCare.Application.Features.Auth.Commands.RegisterPatient;
using HealthCare.Application.Features.Auth.Commands.ConfirmEmail;
using HealthCare.Application.Features.Auth.Commands.ResendConfirmationEmail;
using HealthCare.Application.Features.Auth.Commands.ForgotPassword;
using HealthCare.Application.Features.Auth.Commands.ResetPassword;
using HealthCare.Application.Features.Auth.Commands.VerifyResetPasswordOtp;
using HealthCare.Application.Features.Auth.Commands.RefreshToken;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HealthCare.Application.Features.Auth.Commands.ChangePassword;
using HealthCare.Application.Features.Auth.Contracts;

namespace HealthCare.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(ISender mediatr) : ControllerBase
{
    private readonly ISender _mediatr = mediatr;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        var res = await _mediatr.Send(command, cancellationToken);
        return res.IsSuccess ? Ok(res.Value) : res.ToProblem();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterPatientCommand command, CancellationToken cancellationToken)
    {
        var res = await _mediatr.Send(command, cancellationToken);
        return res.IsSuccess ? Ok(res.Value) : res.ToProblem();
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailCommand command, CancellationToken cancellationToken)
    {
        var res = await _mediatr.Send(command, cancellationToken);
        return res.IsSuccess ? Ok() : res.ToProblem();
    }

    [HttpPost("resend-confirmation-email")]
    public async Task<IActionResult> ResendConfirmation([FromBody] ResendConfirmationEmailCommand command, CancellationToken cancellationToken)
    {
        var res = await _mediatr.Send(command, cancellationToken);
        return res.IsSuccess ? Ok() : res.ToProblem();
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command, CancellationToken cancellationToken)
    {
        var res = await _mediatr.Send(command, cancellationToken);
        return res.IsSuccess ? Ok() : res.ToProblem();
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        var res = await _mediatr.Send(command, cancellationToken);
        return res.IsSuccess ? Ok() : res.ToProblem();
    }

    [HttpPost("verify-reset-otp")]
    public async Task<IActionResult> VerifyResetOtp([FromBody] VerifyResetPasswordOtpCommand command, CancellationToken cancellationToken)
    {
        var res = await _mediatr.Send(command, cancellationToken);
        return res.IsSuccess ? Ok() : res.ToProblem();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediatr.Send(command, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [Authorize]
    [HttpPost("Change-Password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request ,CancellationToken cancellationToken)
    {
        var command = new ChangePasswordCommand(User.GetUserId()!, request.CurrentPassword, request.NewPassword);

        var result = await _mediatr.Send(command , cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

}
