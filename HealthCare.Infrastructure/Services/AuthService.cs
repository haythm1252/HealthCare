using Hangfire;
using HealthCare.Application.Common.Consts;
using HealthCare.Application.Common.Helpers;
using HealthCare.Application.Common.Result;
using HealthCare.Application.Common.Settings;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.Auth.Commands.RegisterPatient;
using HealthCare.Application.Features.Auth.Contracts;
using HealthCare.Application.Services;
using HealthCare.Domain.Users;
using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Threading;

namespace HealthCare.Infrastructure.Services;

public class AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailService emailService,
        IJwtService jwtService,
        IWebHostEnvironment env,
        IOptions<JwtSettings> jwtSettings,
        ILogger<AuthService> logger

    ) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly IEmailService _emailService = emailService;
    private readonly IJwtService _jwtService = jwtService;
    private readonly IWebHostEnvironment _env = env;
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;
    private readonly ILogger<AuthService> _logger = logger;


    public async Task<Result<AuthResponse>> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCradentials);

        if (user.IsDisabled)
            return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

        var result = await _signInManager.PasswordSignInAsync(user, password, false, true);
        if (!result.Succeeded)
        {
            var error = result.IsLockedOut ? UserErrors.LockedUser :
                        result.IsNotAllowed ? UserErrors.EmailNotConfirmed :
                        UserErrors.InvalidCradentials;
            return Result.Failure<AuthResponse>(error);
        }

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault();  

        (string token, int exp) = _jwtService.GenerateToken(user, role!);

        var refreshToken = CreateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays);

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            ExpiresOn = refreshTokenExpiration
        });
        await _userManager.UpdateAsync(user);

        var response = new AuthResponse(user.Id, user.Email!, user.Name, role!, token, exp, refreshToken, refreshTokenExpiration);
        return Result.Success(response);

    }

    public async Task<Result<RegisterResponse>> RegisterPatientAsync(RegisterPatientCommand request, CancellationToken cancellationToken = default)
    {
        var user = request.Adapt<ApplicationUser>();
        var res = await _userManager.CreateAsync(user, request.Password);
        if (!res.Succeeded)
        {
            var error = res.Errors.First();
            return Result.Failure<RegisterResponse>(new Error(error.Code, error.Description, 400));
        }

        var roleRes = await _userManager.AddToRoleAsync(user, DefaultRoles.Patient);
        if (!roleRes.Succeeded)
        {
            var error = res.Errors.First();
            return Result.Failure<RegisterResponse>(new Error(error.Code, error.Description, 400));
        }
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        _logger.LogInformation("Generated Email Confirmation Code: {Code}", code);

        await SendconfirmationEmailAsync(user, code, request.CallbackUrl);
        var response = new RegisterResponse(user.Id, user.Email!, DefaultRoles.Patient, $"User Register successfully, Confirmation Email Sent to {user.Email}");

        return Result.Success(response);
    }
    public async Task<Result> ConfirmEmailAsync(string userId, string token, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return Result.Failure(UserErrors.InvalidCradentials);

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailConfirmed);

        try
        {
            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        }
        catch (FormatException)
        {
            return Result.Failure(UserErrors.InvalidCode);
        }
        var res = await _userManager.ConfirmEmailAsync(user, token);
        if (!res.Succeeded)
        {
            var error = res.Errors.FirstOrDefault();
            return Result.Failure(new Error(error!.Code, error.Description, 400));
        }
        return Result.Success();
    }

    public async Task<Result> ChangePasswordAsync(string userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return Result.Failure(UserErrors.NotFound);

        var res = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (!res.Succeeded)
        {
            var error = res.Errors.FirstOrDefault();
            return Result.Failure(new Error(error!.Code, error.Description, 400));
        }

        return Result.Success();
    }
    
    public async Task<Result> ResendConfirmationEmailAsync(string email, string callbackUrl, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return Result.Failure(UserErrors.NotFound);

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailConfirmed);

        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        _logger.LogInformation("Generated Email Confirmation Code (resend): {Code}", code);

        await SendconfirmationEmailAsync(user, code, callbackUrl);
        return Result.Success();
    }
    
    public async Task<Result> ForgotPasswordAsync(string email, string callbackUrl, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return Result.Failure(UserErrors.NotFound);

        if (user.EmailConfirmed == false)
            return Result.Failure(UserErrors.EmailNotConfirmed);

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        _logger.LogInformation("Generated Password Reset Token: {Token}", token);

        await SendPasswordResetEmailAsync(user, token, callbackUrl);
        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(string userId, string token, string newPassword, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return Result.Failure(UserErrors.NotFound);

        try
        {
            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        }
        catch (FormatException)
        {
            return Result.Failure(UserErrors.InvalidCode);
        }

        var res = await _userManager.ResetPasswordAsync(user, token, newPassword);
        if (!res.Succeeded)
        {
            var error = res.Errors.FirstOrDefault();
            return Result.Failure(new Error(error!.Code, error.Description, 400));
        }

        return Result.Success();
    }
    public async Task<bool> IsUserExist(string email, CancellationToken cancellationToken = default) 
        => await _userManager.Users.AnyAsync(u => u.Email == email, cancellationToken);
    public async Task<ApplicationUser?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
        => await _userManager.FindByIdAsync(userId);

    private async Task SendconfirmationEmailAsync(ApplicationUser user, string code, string callbackUrl)
    {
        var template = Path.Combine(_env.WebRootPath, "EmailTemplates", "ConfirmationEmail.html");
        var emailBody = EmailBodyBuilder.BuildEmailBody(template,
            new Dictionary<string, string>
            {
                {"{{UserName}}",$"{user.Name}" },
                {"{{ConfirmationLink}}",$"{callbackUrl}?userId={user.Id}&code={code}" }
            });
       BackgroundJob.Enqueue(() => _emailService.SendEmailAsync(user.Email!, "Confirm your email", emailBody));
    }
    
    private async Task SendPasswordResetEmailAsync(ApplicationUser user, string token, string callbackUrl)
    {
        var template = Path.Combine(_env.WebRootPath, "EmailTemplates", "PasswordReset.html");
        var resetLink = $"{callbackUrl}?userId={user.Id}&token={token}";
        var emailBody = EmailBodyBuilder.BuildEmailBody(template,
            new Dictionary<string, string>
            {
                {"{{UserName}}", user.Name },
                {"{{ResetLink}}", resetLink }
            });
        BackgroundJob.Enqueue(() => _emailService.SendEmailAsync(user.Email!, "Reset your password", emailBody));
    }

    private static string CreateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));


}

