using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Auth.Commands.RegisterPatient;
using HealthCare.Application.Features.Auth.Contracts;
using HealthCare.Application.Features.Users.Commands.MedicalStaffRegister;
using HealthCare.Application.Interfaces.Repositories;
using HealthCare.Domain.Users;

public interface IAuthService 
{
    Task<Result<AuthResponse>> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<Result<RegisterResponse>> RegisterPatientAsync(RegisterPatientCommand request, CancellationToken cancellationToken = default);
    Task<Result> ConfirmEmailAsync(string email, string otp, CancellationToken cancellationToken = default);
    Task<Result> ResendConfirmationEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Result> ForgotPasswordAsync(string email, CancellationToken cancellationToken = default);
    Task<Result> ResetPasswordAsync(string email, string otp, string newPassword, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
    Task<Result> ChangePasswordAsync(string userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);
    Task<Result> VerifyResetPasswordOtpAsync(string email, string otp, CancellationToken cancellationToken = default);
    Task<Result<RegisterResponse>> RegisterMedicalStaffAsync(MedicalStaffRegisterCommand request, CancellationToken cancellationToken = default);
    Task<bool> IsUserExist(string email, CancellationToken cancellationToken = default);
    Task<ApplicationUser?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);
}