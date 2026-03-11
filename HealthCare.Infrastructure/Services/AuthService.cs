using Hangfire;
using HealthCare.Application.Common.Consts;
using HealthCare.Application.Common.Helpers;
using HealthCare.Application.Common.Result;
using HealthCare.Application.Common.Settings;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.Auth.Commands.RegisterPatient;
using HealthCare.Application.Features.Auth.Contracts;
using HealthCare.Application.Features.Users.Commands.MedicalStaffRegister;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Application.Services;
using HealthCare.Domain.Entities;
using HealthCare.Domain.Enums;
using HealthCare.Domain.Users;
using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using static System.Net.WebRequestMethods;

namespace HealthCare.Infrastructure.Services;

public class AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailService emailService,
        IUnitOfWork unitOfWork,
        IJwtService jwtService,
        IWebHostEnvironment env,
        IOptions<JwtSettings> jwtSettings,
        ILogger<AuthService> logger

    ) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly IEmailService _emailService = emailService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IJwtService _jwtService = jwtService;
    private readonly IWebHostEnvironment _env = env;
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;
    private readonly ILogger<AuthService> _logger = logger;


    // Login and Refersh token
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
        user.LastLogin = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);

        var response = new AuthResponse(user.Id, user.Email!, user.Name, role!, token, exp, refreshToken, refreshTokenExpiration);
        return Result.Success(response);

    }
    public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtService.ValidateToken(token);
        if (userId is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidToken);

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidToken);

        if (user.IsDisabled)
            return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

        var refershTokenData = user.RefreshTokens.FirstOrDefault(x => x.Token == refreshToken && x.IsActive);
        if (refershTokenData is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidRefreshToken);

        refershTokenData.RevokedOn = DateTime.UtcNow;

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault();

        (string jwtToken, int exp) = _jwtService.GenerateToken(user, role!);
        var newRefreshToken = CreateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays);

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = newRefreshToken,
            ExpiresOn = refreshTokenExpiration
        });

        await _userManager.UpdateAsync(user);

        var response = new AuthResponse(user.Id, user.Email!, user.Name, role!, jwtToken, exp, newRefreshToken, refreshTokenExpiration);
        return Result.Success(response);
    }


    // Registration
    public async Task<Result<RegisterResponse>> RegisterMedicalStaffAsync(MedicalStaffRegisterCommand request, CancellationToken cancellationToken = default)
    {
        var user = request.Adapt<ApplicationUser>();
        var password = "@Password" + new Random().Next(1000, 9999);

        var res = await _userManager.CreateAsync(user, password);
        if (!res.Succeeded)
        {
            var error = res.Errors.First();
            return Result.Failure<RegisterResponse>(new Error(error.Code, error.Description, 400));
        }

        var roleRes = await _userManager.AddToRoleAsync(user, request.Role);
        if (!roleRes.Succeeded)
        {
            var error = roleRes.Errors.First();
            return Result.Failure<RegisterResponse>(new Error(error.Code, error.Description, 400));
        }

        user.EmailConfirmed = true;
        await _userManager.UpdateAsync(user);

        await SendMedicalAccountCreatedEmail(user, password);

        var response = new RegisterResponse(user.Id, user.Email!, request.Role, "User Register successfully");
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
        
        var otp = GenerateOtpCode();
        var emailOtp = new EmailOtp
        {
            UserId = user.Id,
            OtpCode = otp
        };

        await _unitOfWork.EmailOtps.AddAsync(emailOtp, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await SendconfirmationEmailAsync(user, otp);
        var response = new RegisterResponse(user.Id, user.Email!, DefaultRoles.Patient, $"User Register successfully, Email Confirmation Otp Sent to {user.Email}");

        return Result.Success(response);
    }



    // Email confirmation
    public async Task<Result> ConfirmEmailAsync(string email, string otp, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return Result.Failure(UserErrors.InvalidCradentials);

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailConfirmed);

        var emailOtp = await _unitOfWork.EmailOtps
            .AsQueryable()
            .Where(x => x.UserId == user.Id && x.OtpCode == otp)
            .SingleOrDefaultAsync(cancellationToken);

        if (emailOtp is null)
            return Result.Failure(UserErrors.InvalidOtp);

        if (emailOtp.ExpirationTime < DateTime.UtcNow)
            return Result.Failure(UserErrors.OtpExpired);

        //confirm email and delete all otps of the user
        user.EmailConfirmed = true;
        await _userManager.UpdateAsync(user);

        await _unitOfWork.EmailOtps
            .AsQueryable()
            .Where(x => x.UserId == user.Id)
            .ExecuteDeleteAsync(cancellationToken);

        return Result.Success();
    }
    public async Task<Result> ResendConfirmationEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return Result.Failure(UserErrors.NotFound);

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailConfirmed);

        await _unitOfWork.EmailOtps
            .AsQueryable()
            .Where(x => x.UserId == user.Id)
            .ExecuteDeleteAsync(cancellationToken);

        var otp = GenerateOtpCode();
        var emailOtp = new EmailOtp
        {
            UserId = user.Id,
            OtpCode = otp
        };

        await _unitOfWork.EmailOtps.AddAsync(emailOtp, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await SendconfirmationEmailAsync(user, otp);
        return Result.Success();
    }


    // Password management
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
    
    public async Task<Result> ForgotPasswordAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return Result.Failure(UserErrors.NotFound);

        if (user.EmailConfirmed == false)
            return Result.Failure(UserErrors.EmailNotConfirmed);

        await _unitOfWork.EmailOtps
            .AsQueryable()
            .Where(x => x.UserId == user.Id)
            .ExecuteDeleteAsync(cancellationToken);

        var otp = GenerateOtpCode();
        var emailOtp = new EmailOtp
        {
            UserId = user.Id,
            OtpCode = otp
        };

        await _unitOfWork.EmailOtps.AddAsync(emailOtp, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await SendPasswordResetEmailAsync(user, otp);
        return Result.Success();
    }
    public async Task<Result> VerifyResetPasswordOtpAsync(string email, string otp, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return Result.Failure(UserErrors.NotFound);

        var emailOtp = await _unitOfWork.EmailOtps
            .AsQueryable()
            .Where(x => x.UserId == user.Id && x.OtpCode == otp)
            .SingleOrDefaultAsync(cancellationToken);

        if (emailOtp is null)
            return Result.Failure(UserErrors.InvalidOtp);

        if (emailOtp.ExpirationTime < DateTime.UtcNow)
            return Result.Failure(UserErrors.OtpExpired);

        return Result.Success();
    }
    public async Task<Result> ResetPasswordAsync(string email, string otp, string newPassword, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return Result.Failure(UserErrors.NotFound);

        var emailOtp = await _unitOfWork.EmailOtps
            .AsQueryable()
            .Where(x => x.UserId == user.Id && x.OtpCode == otp)
            .SingleOrDefaultAsync(cancellationToken);

        if (emailOtp is null)
            return Result.Failure(UserErrors.InvalidOtp);

        if (emailOtp.ExpirationTime < DateTime.UtcNow)
            return Result.Failure(UserErrors.OtpExpired);

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var res = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
        if (!res.Succeeded)
        {
            var error = res.Errors.FirstOrDefault();
            return Result.Failure(new Error(error!.Code, error.Description, 400));
        }

        await _unitOfWork.EmailOtps
            .AsQueryable()
            .Where(x => x.UserId == user.Id)
            .ExecuteDeleteAsync(cancellationToken);


        return Result.Success();
    }


    // application user methods
    public async Task<bool> IsUserExist(string email, CancellationToken cancellationToken = default) 
        => await _userManager.Users.AnyAsync(u => u.Email == email, cancellationToken);
    public async Task<ApplicationUser?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
        => await _userManager.FindByIdAsync(userId);



    // Email sending
    private async Task SendconfirmationEmailAsync(ApplicationUser user, string otp)
    {
        string emailBody;
        try
        {
            var template = Path.Combine(_env.WebRootPath, "EmailTemplates", "ConfirmationEmail.html");
            emailBody = EmailBodyBuilder.BuildEmailBody(template,
                new Dictionary<string, string>
                {
                {"{{UserName}}", user.Name },
                {"{{OTP}}", otp }
                });
        }
        catch
        {
            // fallback: plain text email
            emailBody = $"Hi {user.Name},\nYour confirmation code is: {otp}\nPlease use it to confirm your email.";
        }

        BackgroundJob.Enqueue(() => _emailService.SendEmailAsync(user.Email!, "Confirm your email", emailBody));
    }
    private async Task SendPasswordResetEmailAsync(ApplicationUser user, string otp)
    {
        string emailBody;
        try
        {
            var template = Path.Combine(_env.WebRootPath, "EmailTemplates", "PasswordReset.html");
            emailBody = EmailBodyBuilder.BuildEmailBody(template,
                new Dictionary<string, string>
                {
                {"{{UserName}}", user.Name },
                {"{{OTP}}", otp }
                });
        }
        catch
        {
            emailBody = $"Hi {user.Name},\nYour password reset code is: {otp}\nUse it to reset your password.";
        }

        BackgroundJob.Enqueue(() => _emailService.SendEmailAsync(user.Email!, "Reset your password", emailBody));
    }
    private async Task SendMedicalAccountCreatedEmail(ApplicationUser user, string password)
    {
        string emailBody;
        try
        {
            var template = Path.Combine(_env.WebRootPath, "EmailTemplates", "MedicalAccountCreated.html");
            emailBody = EmailBodyBuilder.BuildEmailBody(template,
                new Dictionary<string, string>
                {
                {"{{UserName}}", user.Name },
                {"{{Email}}", user.Email! },
                {"{{Password}}", password }
                });
        }
        catch
        {
            emailBody = $"Hi {user.Name},\nYour medical account was created.\nEmail: {user.Email}\nPassword: {password}\nPlease change your password after first login or use forgot password if needed.";
        }

        BackgroundJob.Enqueue(() => _emailService.SendEmailAsync(user.Email!, "Medical Account Created", emailBody));
    }

    // Helpers
    private static string CreateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    private static string GenerateOtpCode() => RandomNumberGenerator.GetInt32(100000, 999999).ToString();

}

